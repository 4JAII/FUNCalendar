using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using FUNCalendar.Models;
using FUNCalendar.Services;
using Reactive.Bindings;
using System.Reactive.Disposables;
using Reactive.Bindings.Extensions;
using Prism.Navigation;
using Microsoft.Practices.Unity;
using Prism.Services;
using Xamarin.Forms;

namespace FUNCalendar.ViewModels
{
    public class ToDoListPageViewModel : BindableBase, INavigationAware, IDisposable
    {
        private IToDoList _todoList;
        private IPageDialogService _pageDialogService;
        private INavigationService _navigationService;
        private IStorageService _storageService;

        /* Picker用のソートアイテム */
        public ToDoListSortName[] SortNames { get; private set; }
        /* 表示用リスト */
        public ReadOnlyReactiveCollection<VMToDoItem> DisplayToDoList { get; private set; }
        /* 選ばれたのはこのソートでした */
        public ReactiveProperty<ToDoListSortName> SelectedSortName { get; private set; }
        /* 昇順降順関係 */
        private string order = "昇順";
        public string Order
        {
            get { return this.order; }
            set { this.SetProperty(ref this.order, value); }
        }
        public ReactiveCommand OrderChangeCommand { get; private set; }
        /* 画面遷移用 */
        public AsyncReactiveCommand NavigationRegisterPageCommand { get; private set; }
        /* 削除用 */
        public AsyncReactiveCommand<object> DeleteToDoItemCommand { get; private set; } = new AsyncReactiveCommand();
        /* 編集用 */
        public AsyncReactiveCommand<object> EditToDoItemCommand { get; private set; } = new AsyncReactiveCommand();
        /* 購読解除用 */
        private CompositeDisposable disposable { get; } = new CompositeDisposable();



        public ToDoListPageViewModel(IToDoList todoList, IStorageService storageService,INavigationService navigationService, IPageDialogService pageDialogService)
        {
            this._todoList = todoList;
            this._storageService = storageService;
            this._pageDialogService = pageDialogService;
            this._navigationService = navigationService;
            OrderChangeCommand = new ReactiveCommand();
            NavigationRegisterPageCommand = new AsyncReactiveCommand();
            SelectedSortName = new ReactiveProperty<ToDoListSortName>();

            /* ToDoItemをVMToDoItemに変換しつつReactiveCollection化 */
            DisplayToDoList = _todoList.SortedToDoList.ToReadOnlyReactiveCollection(x => new VMToDoItem(x)).AddTo(disposable);
            SortNames = new[]{ /* Pickerのアイテムセット */
                new ToDoListSortName
                {
                    SortName = "登録順",
                    Sort = _todoList.SortByID
                },
                new ToDoListSortName
                {
                    SortName = "項目名順",
                    Sort = _todoList.SortByName
                },
                new ToDoListSortName
                {
                    SortName = "予定日順",
                    Sort = _todoList.SortByDate
                },
                new ToDoListSortName
                {
                    SortName = "優先度順",
                    Sort = _todoList.SortByPriority
                }
            };
            SelectedSortName.Value = SortNames[0];

            /* 編集するものをセットして遷移 */
            EditToDoItemCommand.Subscribe(async (obj) =>
            {
                VMToDoItem item = obj as VMToDoItem;
                if (item.WishID != 0)
                {
                    _storageService.WishList.SetDisplayWishItem(item.WishID);
                    await _pageDialogService.DisplayAlertAsync("確認", "WishListと連携しているアイテムなのでWishList編集画面に移動します", "OK");
                    await _navigationService.NavigateAsync($"/NavigationPage/WishListRegisterPage?CanEdit=T");
                }
                else
                {
                    _todoList.SetDisplayToDoItem(VMToDoItem.ToToDoItem(item));
                    await _navigationService.NavigateAsync($"/NavigationPage/ToDoListRegisterPage?CanEdit=T");
                }
            });

            /* 確認して消す */
            DeleteToDoItemCommand.Subscribe(async (obj) =>
            {
                var result = await _pageDialogService.DisplayAlertAsync("確認", "削除しますか？", "はい", "いいえ");
                if (result)
                {
                    var todoItem = VMToDoItem.ToToDoItem(obj as VMToDoItem);
                    bool needsDelete = false;
                    if (todoItem.WishID != 0)
                        needsDelete = await _pageDialogService.DisplayAlertAsync("確認", "関連のWishListも削除しますか？\n(いいえを選んだ場合そのWishListの連携機能が使えなくなります)", "はい", "いいえ");
                    await _storageService.DeleteItem(todoItem, needsDelete);
                }
            });

            /*画面遷移設定*/
            NavigationRegisterPageCommand.Subscribe(async () => await this._navigationService.NavigateAsync($"/NavigationPage/ToDoListRegisterPage"));
            /* 選ばれた並べ替え方法が変わったとき */
            SelectedSortName.Subscribe(_ => { if (_ != null) SelectedSortName.Value.Sort(); }).AddTo(disposable);
            /* 昇順降順が変わった時 */
            OrderChangeCommand.Subscribe(() =>
            {
                _todoList.IsAscending = !_todoList.IsAscending;/* MVVM違反？*/
                Order = _todoList.IsAscending ? "昇順" : "降順";
                SelectedSortName.Value.Sort();
            }).AddTo(disposable);
        }

        public void OnNavigatedFrom(NavigationParameters parameters)
        {
            this.Dispose();
        }

        public void OnNavigatedTo(NavigationParameters parameters)
        {
            _todoList.IsAscending = true;/* MVVM違反？*/

        }

        public void OnNavigatingTo(NavigationParameters parameters)
        {

        }

        public void Dispose()
        {
            disposable.Dispose();
        }
    }

}