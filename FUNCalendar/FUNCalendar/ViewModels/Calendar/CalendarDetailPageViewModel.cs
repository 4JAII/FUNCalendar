using Prism.Commands;
using Prism.Mvvm;
using Prism.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using FUNCalendar.Models;
using FUNCalendar.Views;
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
using System.Collections.ObjectModel;
using FUNCalendar.Services;

namespace FUNCalendar.ViewModels
{
    public class CalendarDetailPageViewModel : BindableBase, INavigationAware
    {
        private ICalendar _calendar;
        private IWishList _wishList;
        private IToDoList _todoList;
        private IHouseholdAccounts _householdAccounts;
        private INavigationService _navigationService;
        private IPageDialogService _pageDialogService;
        private IStorageService _storageService;

        /* Date詳細画面用 */
        public ReactiveProperty<DateTime> DateData { get; private set; } = new ReactiveProperty<DateTime>();
        public ReadOnlyReactiveCollection<VMWishItem> DisplayWishList { get; private set; }
        public ReadOnlyReactiveCollection<VMToDoItem> DisplayToDoList { get; private set; }
        public ReadOnlyReactiveCollection<VMHouseholdAccountsItem> DisplayHouseHoldAccountsList { get; private set; }

        /* Command関連 */
        public ReactiveCommand BackCommand { get; private set; }
        public ReactiveCommand ToDoListOpenCloseCommand { get; private set; }
        public ReactiveCommand WishListOpenCloseCommand { get; private set; }
        public ReactiveCommand HouseHoldAccountsListOpenCloseCommand { get; private set; }

        /* アイテム追加,編集用 */
        public AsyncReactiveCommand NavigationRegisterPageCommand { get; private set; }
        public ReactiveCommand EditToDoItemCommand { get; private set; }
        public ReactiveCommand DeleteToDoItemCommand { get; private set; }
        public ReactiveCommand EditWishItemCommand { get; private set; }
        public ReactiveCommand DeleteWishItemCommand { get; private set; }
        public ReactiveCommand EditHouseholdAccountsItemCommand { get; private set; }
        public ReactiveCommand DeleteHouseholdAccountsItemCommand { get; private set; }

        /* ListViewの高さ */
        public ReactiveProperty<int> WishListHeight { get; private set; } = new ReactiveProperty<int>(1);
        public ReactiveProperty<int> ToDoListHeight { get; private set; } = new ReactiveProperty<int>(1);
        public ReactiveProperty<int> HouseholdAccountsListHeight { get; private set; } = new ReactiveProperty<int>(1);

        /* ListView開閉用 */
        private bool wishListOpen = false;
        private bool todoListOpen = false;
        private bool householdAccountsListOpen = false;

        /* 廃棄 */
        private CompositeDisposable Disposable { get; } = new CompositeDisposable();

        public CalendarDetailPageViewModel(ICalendar calendar, IWishList wishList, IToDoList todoList, IHouseholdAccounts householdAccounts, INavigationService navigationService, IPageDialogService pageDialogService, IStorageService storageService)
        {
            /* コンストラクタインジェクションされたインスタンスを保持 */
            this._calendar = calendar;
            this._wishList = wishList;
            this._todoList = todoList;
            this._householdAccounts = householdAccounts;
            this._navigationService = navigationService;
            this._pageDialogService = pageDialogService;
            this._storageService = storageService;

            BackCommand = new ReactiveCommand();
            ToDoListOpenCloseCommand = new ReactiveCommand();
            WishListOpenCloseCommand = new ReactiveCommand();
            HouseHoldAccountsListOpenCloseCommand = new ReactiveCommand();

            NavigationRegisterPageCommand = new AsyncReactiveCommand();
            EditToDoItemCommand = new ReactiveCommand();
            DeleteToDoItemCommand = new ReactiveCommand();
            EditWishItemCommand = new ReactiveCommand();
            DeleteWishItemCommand = new ReactiveCommand();
            EditHouseholdAccountsItemCommand = new ReactiveCommand();
            DeleteHouseholdAccountsItemCommand = new ReactiveCommand();

            DisplayWishList = _wishList.WishListForCalendar.ToReadOnlyReactiveCollection(x => new VMWishItem(x)).AddTo(Disposable);
            DisplayToDoList = _todoList.ToDoListForCalendar.ToReadOnlyReactiveCollection(x => new VMToDoItem(x)).AddTo(Disposable);
            DisplayHouseHoldAccountsList = _householdAccounts.HouseholdAccountsListForCalendar.ToReadOnlyReactiveCollection(x => new VMHouseholdAccountsItem(x)).AddTo(Disposable);

            /* DatePicker */
            DateData.Subscribe( _ =>
            {
                _wishList.ClearWishListForCalendar();
                _todoList.ClearToDoListForCalendar();
                _householdAccounts.ClearHouseholdAccountsListForCalendar();
                WishListHeight.Value = 1;
                ToDoListHeight.Value = 1;
                HouseholdAccountsListHeight.Value = 1;
            });

            /* 戻る処理 */
            BackCommand.Subscribe(async () =>
            {
                await _navigationService.NavigateAsync($"/RootPage/NavigationPage/CalendarPage");
            });

            /* List開閉処理 */
            ToDoListOpenCloseCommand.Subscribe(() =>
            {
                if (todoListOpen)
                {
                    _todoList.ClearToDoListForCalendar();
                    ToDoListHeight.Value = 1;
                    todoListOpen = false;
                }
                else
                {
                    _todoList.SetToDoListForCalendar(DateData.Value);
                    ToDoListHeight.Value = (_todoList.ToDoListForCalendar.Count + 1) * 60;
                    todoListOpen = true;
                }
            });

            WishListOpenCloseCommand.Subscribe(() =>
            {
                if (wishListOpen)
                {
                    _wishList.ClearWishListForCalendar();
                    WishListHeight.Value = 1;
                    wishListOpen = false;
                }
                else
                {
                    _wishList.SetWishListForCalendar(DateData.Value);
                    WishListHeight.Value = (_wishList.WishListForCalendar.Count + 1) * 60;
                    wishListOpen = true;
                }
            });

            HouseHoldAccountsListOpenCloseCommand.Subscribe(() =>
            { 
                if (householdAccountsListOpen)
                {
                    _householdAccounts.ClearHouseholdAccountsListForCalendar();
                    HouseholdAccountsListHeight.Value = 1;
                    householdAccountsListOpen = false;
                }
                else
                {
                    _householdAccounts.SetHouseholdAccountsListForCalendar(DateData.Value);
                    HouseholdAccountsListHeight.Value = (_householdAccounts.HouseholdAccountsListForCalendar.Count + 1) * 60;
                    householdAccountsListOpen = true;
                }
                
            });

            /* アイテム編集処理 */
            EditToDoItemCommand.Subscribe(async (obj) =>
            {
                _todoList.SetDisplayToDoItem(VMToDoItem.ToToDoItem(obj as VMToDoItem));
                await _navigationService.NavigateAsync($"/NavigationPage/ToDoListRegisterPage?CanEdit=T");
            });
            EditWishItemCommand.Subscribe(async (obj) =>
            {
                _wishList.SetDisplayWishItem(VMWishItem.ToWishItem(obj as VMWishItem));
                await _navigationService.NavigateAsync($"/NavigationPage/WishListRegisterPage?CanEdit=T");
            });
            EditHouseholdAccountsItemCommand.Subscribe(async (obj) =>
            {
                _householdAccounts.SetHouseholdAccountsItem(VMHouseholdAccountsItem.ToHouseholdaccountsItem(obj as VMHouseholdAccountsItem));
                var navigationitem = new HouseholdAccountsNavigationItem(DateData.Value);
                var navigationparameter = new NavigationParameters()
                {
                    {HouseholdAccountsRegisterPageViewModel.EditKey, navigationitem }
                };
                await _navigationService.NavigateAsync("/NavigationPage/HouseholdAccountsRegisterPage", navigationparameter);
            });

            /* アイテム削除処理 */
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
            DeleteHouseholdAccountsItemCommand.Subscribe(async (obj) =>
            {
                var result = await _pageDialogService.DisplayAlertAsync("確認", "削除しますか？", "はい", "いいえ");
                if (result)
                {
                    await _storageService.DeleteItem(VMHouseholdAccountsItem.ToHouseholdaccountsItem(obj as VMHouseholdAccountsItem));
                    _householdAccounts.SetHouseholdAccountsListForCalendar(DateData.Value);
                }
            });

            /* 画面遷移設定 */
            NavigationRegisterPageCommand.Subscribe(async () =>
            {
                var result = await _pageDialogService.DisplayActionSheetAsync("登録するアイテムの種類を選択", "キャンセル","", "ToDo", "WishList", "家計簿");
                var navigationParameters = new NavigationParameters();
                navigationParameters.Add("DateData", DateData.Value);
                navigationParameters.Add("FromCalendar", "T");
                switch (result)
                {
                    case "ToDo":
                        navigationParameters.Add("BackPage", "/NavigationPage/CalendarDetailPage");
                        await this._navigationService.NavigateAsync($"/NavigationPage/ToDoListRegisterPage" ,navigationParameters);
                        break;
                    case "WishList":
                        navigationParameters.Add("BackPage", "/NavigationPage/CalendarDetailPage");
                        await this._navigationService.NavigateAsync($"/NavigationPage/WishListRegisterPage", navigationParameters);
                        break;
                    case "家計簿":
                        var navigationitem = new HouseholdAccountsNavigationItem(DateData.Value);
                        navigationParameters.Add("BackPage", PageName.CalendarDetailPage);
                        navigationParameters.Add(HouseholdAccountsRegisterPageViewModel.CalendarKey, navigationitem);
                        await this._navigationService.NavigateAsync($"/NavigationPage/HouseholdAccountsRegisterPage", navigationParameters);
                        break;
                }
            });
        }

        public void OnNavigatedFrom(NavigationParameters parameters)
        {
            
        }

        public void OnNavigatedTo(NavigationParameters parameters)
        {
            DateData.Value = _calendar.DisplayDate.DateData;
        }

        public void OnNavigatingTo(NavigationParameters parameters)
        {
            
        }
    }
}
