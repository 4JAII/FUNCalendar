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
    public class WishListPageViewModel : BindableBase, INavigationAware, IDisposable
    {
        private IWishList _wishList;
        private IPageDialogService _pageDialogService;
        private INavigationService _navigationService;
        private IStorageService _storageService;

        /* Picker用のソートアイテム */
        public WishListSortName[] SortNames { get; private set; }
        /* 表示用リスト */
        public ReadOnlyReactiveCollection<VMWishItem> DisplayWishList { get; private set; }
        /* 選ばれたのはこのソートでした */
        public ReactiveProperty<WishListSortName> SelectedSortName { get; private set; }
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
        public ReactiveCommand<object> DeleteWishItemCommand { get; private set; } = new ReactiveCommand();
        /* 編集用 */
        public ReactiveCommand<object> EditWishItemCommand { get; private set; } = new ReactiveCommand();
        /* 購読解除用 */
        private CompositeDisposable disposable { get; } = new CompositeDisposable();



        public WishListPageViewModel(IWishList wishList, IStorageService storageService, INavigationService navigationService, IPageDialogService pageDialogService)
        {
            this._wishList = wishList;
            this._storageService = storageService;
            this._pageDialogService = pageDialogService;
            this._navigationService = navigationService;
            OrderChangeCommand = new ReactiveCommand();
            NavigationRegisterPageCommand = new AsyncReactiveCommand();
            SelectedSortName = new ReactiveProperty<WishListSortName>();

            var navigationParameters = new NavigationParameters();
            navigationParameters.Add("BackPage", "/RootPage/NavigationPage/WishListPage");

            /* WishItemをVMWishItemに変換しつつReactiveCollection化 */
            DisplayWishList = _wishList.SortedWishList.ToReadOnlyReactiveCollection(x => new VMWishItem(x)).AddTo(disposable);
            SortNames = new[]{ /* Pickerのアイテムセット */
                new WishListSortName
                {
                    SortName = "登録順",
                    Sort = _wishList.SortByID
                },
                new WishListSortName
                {
                    SortName = "名前順",
                    Sort = _wishList.SortByName
                },
                new WishListSortName
                {
                    SortName = "値段順",
                    Sort = _wishList.SortByPrice
                },
                new WishListSortName
                {
                    SortName = "購入予定日順",
                    Sort = _wishList.SortByDate
                }
            };
            SelectedSortName.Value = SortNames[0];

            /* 編集するものをセットして遷移 */
            EditWishItemCommand.Subscribe(async (obj) =>
            {
                _wishList.SetDisplayWishItem(VMWishItem.ToWishItem(obj as VMWishItem));
                await _navigationService.NavigateAsync($"/NavigationPage/WishListRegisterPage?CanEdit=T", navigationParameters);
            });

            /* 確認して消す */
            DeleteWishItemCommand.Subscribe(async (obj) =>
            {
                var result = await _pageDialogService.DisplayAlertAsync("確認", "削除しますか？", "はい", "いいえ");
                if (result)
                {
                    var wishItem = VMWishItem.ToWishItem(obj as VMWishItem);
                    bool needsDelete = false;
                    if (wishItem.ToDoID != 0)
                        needsDelete = await _pageDialogService.DisplayAlertAsync("確認", "関連のToDoも削除しますか？\n(いいえを選んだ場合そのToDoの連携機能が使えなくなります)", "はい", "いいえ");
                    await _storageService.DeleteItem(wishItem, needsDelete);
                }
            });

            /*画面遷移設定*/
            NavigationRegisterPageCommand.Subscribe(async () => await this._navigationService.NavigateAsync($"/NavigationPage/WishListRegisterPage", navigationParameters));
            /* 選ばれた並べ替え方法が変わったとき */
            SelectedSortName.Subscribe(_ => { if (_ != null) SelectedSortName.Value.Sort(); }).AddTo(disposable);
            /* 昇順降順が変わった時 */
            OrderChangeCommand.Subscribe(() =>
            {
                _wishList.IsAscending = !_wishList.IsAscending;/* MVVM違反？*/
                Order = _wishList.IsAscending ? "昇順" : "降順";
                SelectedSortName.Value.Sort();
            }).AddTo(disposable);
        }

        public void OnNavigatedFrom(NavigationParameters parameters)
        {
            this.Dispose();
        }

        public void OnNavigatedTo(NavigationParameters parameters)
        {
            _wishList.IsAscending = true;/* MVVM違反？ */

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

