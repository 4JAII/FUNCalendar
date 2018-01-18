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

namespace FUNCalendar.ViewModels
{
    public class CalendarDetailPageViewModel : BindableBase, INavigationAware
    {
        private ICalendar _calendar;
        private IWishList _wishList;
        private INavigationService _navigationService;
        private IPageDialogService _pageDialogService;

        /* Date詳細画面用 */
        public ReactiveProperty<DateTime> DateData { get; private set; } = new ReactiveProperty<DateTime>();
        public ReadOnlyReactiveCollection<VMWishItem> DisplayWishList { get; private set; }
        //public ReadOnlyReactiveCollection<VMToDoItem> DisplayToDoList { get; private set; }
        //public ReadOnlyReactiveCollection<VMHouseHoldAccountsItem> DisplayHouseHoldAccountsList { get; private set; }

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
        public ReactiveCommand EditHouseHoldAccountsItemCommand { get; private set; }
        public ReactiveCommand DeleteHouseHoldAccountsItemCommand { get; private set; }

        /* ListViewの高さ */
        public ReactiveProperty<int> WishListHeight { get; private set; } = new ReactiveProperty<int>(1);
        public ReactiveProperty<int> ToDoListHeight { get; private set; } = new ReactiveProperty<int>(1);
        public ReactiveProperty<int> HouseholdAccountsListHeight { get; private set; } = new ReactiveProperty<int>(1);

        /* 廃棄 */
        private CompositeDisposable Disposable { get; } = new CompositeDisposable();

        public CalendarDetailPageViewModel(ICalendar calendar,IWishList wishList, INavigationService navigationService, IPageDialogService pageDialogService)
        {
            /* コンストラクタインジェクションされたインスタンスを保持 */
            this._calendar = calendar;
            this._wishList = wishList;
            this._navigationService = navigationService;
            this._pageDialogService = pageDialogService;

            BackCommand = new ReactiveCommand();
            ToDoListOpenCloseCommand = new ReactiveCommand();
            WishListOpenCloseCommand = new ReactiveCommand();
            HouseHoldAccountsListOpenCloseCommand = new ReactiveCommand();

            NavigationRegisterPageCommand = new AsyncReactiveCommand();
            EditToDoItemCommand = new ReactiveCommand();
            DeleteToDoItemCommand = new ReactiveCommand();
            EditWishItemCommand = new ReactiveCommand();
            DeleteWishItemCommand = new ReactiveCommand();
            EditHouseHoldAccountsItemCommand = new ReactiveCommand();
            DeleteHouseHoldAccountsItemCommand = new ReactiveCommand();

            DisplayWishList = _wishList.WishListForCalendar.ToReadOnlyReactiveCollection(x => new VMWishItem(x)).AddTo(Disposable);
            //DisplayToDoList = _todoList.ToDoListForCalendar.ToReadOnlyReactiveCollection(x => new VMToDoItem(x)).AddTo(Disposable);
            //DisplayHouseHoldAccountsList = _houseHoldAccountsList.HouseHoldAccountsListForCalendar.ToReadOnlyReactiveCollection(x => new VMHouseHoldAccountsItem(x)).AddTo(Disposable);

            /* DatePicker */
            DateData.Subscribe( _ =>
            {
                _wishList.ClearWishListForCalendar();
                //_todoList.ClearToDoListForCalendar();
                //_houseHoldAccountsList.ClearHouseHoldAcountsListForCalendar();
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
                /*
                if (_todoList.ToDoListForCalendar.Any())
                {
                    _todoList.ClearToDoListForCalendar();
                    ToDoListHeight.Value = 1;
                }
                else
                {
                    _todoList.SetToDoListForCalendar(DateData.Value);
                    ToDoListHeight.Value = (_todoList.ToDoListForCalendar.Count + 1) * 60;
                }
                */
            });

            WishListOpenCloseCommand.Subscribe(() =>
            {
                if (_wishList.WishListForCalendar.Any())
                {
                    _wishList.ClearWishListForCalendar();
                    WishListHeight.Value = 1;
                }
                else
                {
                    _wishList.SetWishListForCalendar(DateData.Value);
                    WishListHeight.Value = (_wishList.WishListForCalendar.Count + 1) * 60;
                }
            });

            HouseHoldAccountsListOpenCloseCommand.Subscribe(() =>
            {
                /*
                if (_houseHoldAccountsList.HouseHoldAccountsListForCalendar.Any())
                {
                    _houseHoldAccountsList.ClearHouseHoldAccountsListForCalendar();
                    HouseholdAccountsListHeight.Value = 1;
                }
                else
                {
                    _houseHoldAccountsList.SetHouseHoldAccountsListForCalendar(DateData.Value);
                    HouseholdAccountsListHeight.Value = (_HouseholdAccountsList.HouseholdAccountsListForCalendar.Count + 1) * 60;
                }
                */
            });

            /* アイテム編集処理 */
            /*EditToDoItemCommand.Subscribe(async (obj) =>
            {
                _todoList.SetDisplayToDoItem(VMToDoItem.ToToDoItem(obj as VMToDoItem));
                await _navigationService.NavigateAsync($"/NavigationPage/ToDoListRegisterPage?CanEdit=T");
            });*/
            EditWishItemCommand.Subscribe(async (obj) =>
            {
                _wishList.SetDisplayWishItem(VMWishItem.ToWishItem(obj as VMWishItem));
                await _navigationService.NavigateAsync($"/NavigationPage/WishListRegisterPage?CanEdit=T");
            });
            /*EditHouseHoldAccountsItemCommand.Subscribe(async (obj) =>
            {
                _houseHoldAccountsList.SetDisplayHouseHoldAccountsItem(VMHouseHoldAccountsItem.ToHouseHoldAccountsItem(obj as VMHouseHoldAccountsItem));
                await _navigationService.NavigateAsync($"/NavigationPage/HouseHoldAccountsListRegisterPage?CanEdit=T");
            });*/

            /* アイテム削除処理 */
            /*DeleteToDoItemCommand.Subscribe(async (obj) =>
            {
                var result = await _pageDialogService.DisplayAlertAsync("確認", "削除しますか？", "はい", "いいえ");
                if (result)
                {
                    var todoItem = VMToDOItem.ToToDoItem(obj as VMToDoItem);
                    _todoList.Remove(todoItem);
                    await localStorage.DeleteItem(todoItem);
                }
            });*/
            /*DeleteWishItemCommand.Subscribe(async (obj) =>
            {
                var result = await _pageDialogService.DisplayAlertAsync("確認", "削除しますか？", "はい", "いいえ");
                if (result)
                {
                    var wishItem = VMWishItem.ToWishItem(obj as VMWishItem);
                    _wishList.Remove(wishItem);
                    await localStorage.DeleteItem(wishItem);
                }
            });*/
            /*DeleteWishItemCommand.Subscribe(async (obj) =>
            {
                var result = await _pageDialogService.DisplayAlertAsync("確認", "削除しますか？", "はい", "いいえ");
                if (result)
                {
                    var wishItem = VMWishItem.ToWishItem(obj as VMWishItem);
                    _wishList.Remove(wishItem);
                    await localStorage.DeleteItem(wishItem);
                }
            });*/

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
                        await this._navigationService.NavigateAsync($"/NavigationPage/ToDoListRegisterPage" ,navigationParameters);
                        break;
                    case "WishList":
                        await this._navigationService.NavigateAsync($"/NavigationPage/WishListRegisterPage", navigationParameters);
                        break;
                    case "家計簿":
                        await this._navigationService.NavigateAsync($"/NavigationPage/HouseholdAccountsListRegisterPage", navigationParameters);
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
