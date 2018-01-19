using Prism.Commands;
using Prism.Mvvm;
using Prism.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using FUNCalendar.Models;
using FUNCalendar.Services;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using Reactive.Bindings.Binding;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Xamarin.Forms;
using Microsoft.Practices.Unity;
using Prism.Navigation;
using System.Text.RegularExpressions;
using System.Reactive.Disposables;

namespace FUNCalendar.ViewModels
{
    public class ToDoListRegisterPageViewModel : BindableBase, INavigationAware
    {
        private IToDoList _todoList;
        private INavigationService _navigationService;
        private IPageDialogService _pageDialogService;
        private IStorageService _storageService;

        /* ToDoItem登録用 */
        public int ID { get; private set; } = -1;
        [Required(ErrorMessage = "項目名がありません"), StringLength(32)]
        public ReactiveProperty<string> Description { get; private set; } = new ReactiveProperty<string>();
        [Required(ErrorMessage = "日付がありません")]
        public ReactiveProperty<DateTime> Date { get; private set; } = new ReactiveProperty<DateTime>();
        [Required(ErrorMessage = "優先度がありません")]
        public ReactiveProperty<int> Priority { get; private set; } = new ReactiveProperty<int>();
        public bool NeedsAdd { get; set; } = false;
        private string isCompleted;
        private int wishID;

        /* 登録・キャンセルするときの処理用 */
        public ReactiveProperty<bool> CanRegister { get; private set; }
        public AsyncReactiveCommand RegisterToDoItemCommand { get; private set; }
        public AsyncReactiveCommand CancelCommand { get; private set; }

        /* エラー時の色 */
        public ReactiveProperty<Color> ErrorColor { get; private set; } = new ReactiveProperty<Color>();

        /* 廃棄 */
        private CompositeDisposable disposable { get; } = new CompositeDisposable();


        public ToDoListRegisterPageViewModel(IToDoList todoList, IStorageService storageService, INavigationService navigationService, IPageDialogService pageDialogService)
        {
            this._storageService = storageService;

            /* コンストラクタインジェクションされたインスタンスを保持 */
            this._todoList = todoList;
            this._navigationService = navigationService;
            this._pageDialogService = pageDialogService;
            /* 属性を有効化 */
            Description.SetValidateAttribute(() => this.Description);
            Date.SetValidateAttribute(() => this.Date);
            Priority.SetValidateAttribute(() => this.Priority);

            Date.Value = DateTime.Now;/* とりあえず現在の日付に合わせておく */
            Priority.Value = 3;/* とりあえず3に合わせておく */

            /* 全てにエラーなしなら登録できるようにする */
            CanRegister = new[]
            {
                this.Description.ObserveHasErrors,
                this.Date.ObserveHasErrors,
                this.Priority.ObserveHasErrors,
            }
            .CombineLatestValuesAreAllFalse()
            .ToReactiveProperty<bool>();

            /* 登録して遷移 */
            RegisterToDoItemCommand = CanRegister.ToAsyncReactiveCommand();
            RegisterToDoItemCommand.Subscribe(async () =>
            {
                if (ID != -1)
                {
                    var vmToDoItem = new VMToDoItem(ID, Description.Value, Date.Value, Priority.Value.ToString(), isCompleted, wishID);
                    var todoItem = VMToDoItem.ToToDoItem(vmToDoItem);


                    await _storageService.EditItem(_todoList.DisplayToDoItem, todoItem);
                }
                else
                {
                    var todoItem = new ToDoItem { Description = this.Description.Value, Date = Date.Value, Priority = this.Priority.Value, IsCompleted = false, WishID = 0 };
                    await _storageService.AddItem(todoItem);
                }
                await _navigationService.NavigateAsync($"/RootPage/NavigationPage/ToDoListPage");
            });

            /* 登録をキャンセルして遷移(確認もあるよ)  */
            CancelCommand = new AsyncReactiveCommand();
            CancelCommand.Subscribe(async () =>
            {
                var result = await _pageDialogService.DisplayAlertAsync("確認", "入力をキャンセルし画面を変更します。よろしいですか？", "はい", "いいえ");
                if (result) await _navigationService.NavigateAsync($"/RootPage/NavigationPage/ToDoListPage");
            });

            /* 登録できるなら水色,エラーなら灰色 */
            CanRegister.Subscribe(x =>
            {
                ErrorColor.Value = x ? Color.SkyBlue : Color.Gray;
            });
        }

        public void OnNavigatedFrom(NavigationParameters parameters)
        {

        }

        public void OnNavigatedTo(NavigationParameters parameters)
        {
            /* 編集目的で遷移してきたならセット */
            if (parameters["CanEdit"] as string != "T") return;
            VMToDoItem vmToDoItem = new VMToDoItem(_todoList.DisplayToDoItem);
            ID = vmToDoItem.ID;
            Description.Value = vmToDoItem.Description;
            Date.Value = _todoList.DisplayToDoItem.Date;
            Priority.Value = int.Parse(vmToDoItem.Priority);
            isCompleted = vmToDoItem.IsCompleted;
            wishID = vmToDoItem.WishID;
        }

        public void OnNavigatingTo(NavigationParameters parameters)
        {

        }
    }
}
