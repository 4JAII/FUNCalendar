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

        /* 完了コマンド */
        public ReactiveCommand CompleteCommand { get; private set; } = new ReactiveCommand();

        /* 購入済みコマンド */
        public AsyncReactiveCommand BoughtCommand { get; private set; } = new AsyncReactiveCommand();

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
            DateData.Subscribe(_ =>
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
                ToDoItem item = VMToDoItem.ToToDoItem(obj as VMToDoItem);
                var navigationParameters = new NavigationParameters();
                navigationParameters.Add("CanEdit", "T");
                navigationParameters.Add("BackPage", "/RootPage/NavigationPage/CalendarDetailPage");
                if (item.WishID == 0)
                {
                    
                    _todoList.SetDisplayToDoItem(item);
                    await _navigationService.NavigateAsync($"/NavigationPage/ToDoListRegisterPage", navigationParameters);
                }
                else
                {
                    _wishList.SetDisplayWishItem(item.WishID);
                    await _pageDialogService.DisplayAlertAsync("確認", "WishListと連携しているアイテムなのでWishList編集画面に移動します", "OK");
                    await _navigationService.NavigateAsync($"/NavigationPage/WishListRegisterPage", navigationParameters);
                }
            });
            EditWishItemCommand.Subscribe(async (obj) =>
            {
                var navigationParameters = new NavigationParameters();
                navigationParameters.Add("CanEdit", "T");
                navigationParameters.Add("BackPage", "/RootPage/NavigationPage/CalendarDetailPage");
                _wishList.SetDisplayWishItem(VMWishItem.ToWishItem(obj as VMWishItem));
                await _navigationService.NavigateAsync($"/NavigationPage/WishListRegisterPage",navigationParameters);
            });
            EditHouseholdAccountsItemCommand.Subscribe(async (obj) =>
            {
                _householdAccounts.SetHouseholdAccountsItem(VMHouseholdAccountsItem.ToHouseholdaccountsItem(obj as VMHouseholdAccountsItem));
                var navigationitem = new HouseholdAccountsNavigationItem(DateData.Value);
                var navigationparameter = new NavigationParameters();
                navigationparameter.Add("BackPage", PageName.CalendarDetailPage);
                navigationparameter.Add(HouseholdAccountsRegisterPageViewModel.CalendarEditKey, navigationitem);
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
                _todoList.SetToDoListForCalendar(DateData.Value);
                _wishList.SetWishListForCalendar(DateData.Value);
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
                _todoList.SetToDoListForCalendar(DateData.Value);
                _wishList.SetWishListForCalendar(DateData.Value);
            });
            DeleteHouseholdAccountsItemCommand.Subscribe(async (obj) =>
            {
                var result = await _pageDialogService.DisplayAlertAsync("確認", "削除しますか？", "はい", "いいえ");
                if (result)
                {
                    await _storageService.DeleteItem(VMHouseholdAccountsItem.ToHouseholdaccountsItem(obj as VMHouseholdAccountsItem));
                    _householdAccounts.SetHouseholdAccountsListForCalendar(DateData.Value);
                }
                _householdAccounts.SetHouseholdAccountsListForCalendar(DateData.Value);/*未解決*/
            });

            /* 画面遷移設定 */
            NavigationRegisterPageCommand.Subscribe(async () =>
            {
                var result = await _pageDialogService.DisplayActionSheetAsync("登録するアイテムの種類を選択", "キャンセル", "", "ToDo", "WishList", "家計簿");
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

            /* 完了ボタンが押されたときの処理 */
            CompleteCommand.Subscribe(async (obj) =>
            {
                var todoitem = VMToDoItem.ToToDoItem((obj as VMToDoItem));
                /* 完了済みの場合 */
                if (todoitem.IsCompleted)
                {
                    await _pageDialogService.DisplayAlertAsync("注意", "このアイテムは既に完了済みです。", "確認");
                }
                /* 完了済みでない場合 */
                else
                {
                    /* wishIDを持っている場合 */
                    if ((obj as VMToDoItem).WishID != 0)/* wishIDを持っているか */
                    {
                        /* ダイアログで家計簿に登録するか確認 */
                        var result = await _pageDialogService.DisplayAlertAsync("確認", "このアイテムを家計簿に登録しますか？", "はい", "いいえ");

                        /* ダイアログの確認で家計簿に登録するを選択した場合 */
                        if (result)
                        {
                            string scategory, dcategory, storagetype;
                            scategory = await _pageDialogService.DisplayActionSheetAsync("概要カテゴリーの設定", null, null, "食費", "日用雑貨", "交通費", "娯楽費", "医療費", "通信費", "水道_光熱費", "その他_支出");
                            switch (scategory)
                            {
                                case "食費":
                                    dcategory = await _pageDialogService.DisplayActionSheetAsync("詳細カテゴリーの設定", null, null, "食料品", "朝食", "昼食", "夕食", "その他_食費");
                                    break;
                                case "日用雑貨":
                                    dcategory = await _pageDialogService.DisplayActionSheetAsync("詳細カテゴリーの設定", null, null, "消耗品", "子供関連", "ペット関連", "その他_日用雑貨");
                                    break;
                                case "交通費":
                                    dcategory = await _pageDialogService.DisplayActionSheetAsync("詳細カテゴリーの設定", null, null, "電車", "タクシー", "バス", "飛行機", "その他_交通費");
                                    break;
                                case "娯楽費":
                                    dcategory = await _pageDialogService.DisplayActionSheetAsync("詳細カテゴリーの設定", null, null, "レジャー", "イベント", "映画", "音楽", "漫画", "書籍", "ゲーム", "その他_娯楽費");
                                    break;
                                case "医療費":
                                    dcategory = await _pageDialogService.DisplayActionSheetAsync("詳細カテゴリーの設定", null, null, "病院代", "薬代", "生命保険", "医療保険", "その他_医療費");
                                    break;
                                case "通信費":
                                    dcategory = await _pageDialogService.DisplayActionSheetAsync("詳細カテゴリーの設定", null, null, "携帯電話料金", "固定電話料金", "インターネット関連", "テレビ受信料", "宅配便", "切手_はがき", "その他_通信費");
                                    break;
                                case "水道_光熱費":
                                    dcategory = await _pageDialogService.DisplayActionSheetAsync("詳細カテゴリーの設定", null, null, "水道料金", "電気料金", "ガス料金", "その他_水道_光熱費");
                                    break;
                                case "その他_支出":
                                    dcategory = await _pageDialogService.DisplayActionSheetAsync("詳細カテゴリーの設定", null, null, "仕送り", "お小遣い", "使途不明金", "立替金", "その他_支出");
                                    break;
                                default:
                                    dcategory = "食料品"; //とりあえず代入
                                    break;
                            }
                            storagetype = await _pageDialogService.DisplayActionSheetAsync("出金元の設定", null, null, "財布", "貯金", "銀行", "クレジットカード", "その他");
                            if (scategory == null || storagetype == null)
                                return;
                            var Scategory = (SCategorys)Enum.Parse(typeof(SCategorys), scategory);
                            var Dcategory = (DCategorys)Enum.Parse(typeof(DCategorys), dcategory);
                            var Storagetype = (StorageTypes)Enum.Parse(typeof(StorageTypes), storagetype);
                            await _storageService.CompleteToDo(todoitem, true, result, Scategory, Dcategory, Storagetype);

                        }
                        /* 家計簿に登録しない場合 */
                        else
                        {
                            /* とりあえず代入 */
                            var Scategory = SCategorys.start_of_支出;
                            var Dcategory = DCategorys.start_of_その他_支出;
                            var Storagetype = StorageTypes.start_of_Stype;
                            await _storageService.CompleteToDo(todoitem, true, false, Scategory, Dcategory, Storagetype);
                        }
                    }
                    /* wishIDを持っていない場合 */
                    else
                    {
                        /* とりあえず代入 */
                        var Scategory = SCategorys.start_of_支出;
                        var Dcategory = DCategorys.start_of_その他_支出;
                        var Storagetype = StorageTypes.start_of_Stype;
                        await _storageService.CompleteToDo(todoitem, false, false, Scategory, Dcategory, Storagetype);
                    }
                }
            });


            /*購入完了ボタンが押されたときの処理 */
            BoughtCommand.Subscribe(async (obj) =>
            {
                var wishitem = VMWishItem.ToWishItem((obj as VMWishItem));
                /* 購入済みの場合 */
                if (wishitem.IsBought)
                {
                    await _pageDialogService.DisplayAlertAsync("注意", "このアイテムは既に購入済みです", "確認");
                }
                /* 購入済みでなかった時 */
                else
                {
                    var hasID = false;
                    /* todoidを持っていたら */
                    if ((obj as VMWishItem).ToDoID != 0)
                    {
                        hasID = true;
                    }

                    /* ダイアログで家計簿に登録するか確認 */
                    var result = await _pageDialogService.DisplayAlertAsync("確認", "このアイテムを家計簿に登録しますか？", "はい", "いいえ");

                    /* ダイアログの確認で家計簿に登録するを選択した場合 */
                    if (result)
                    {
                        string scategory, dcategory, storagetype;
                        scategory = await _pageDialogService.DisplayActionSheetAsync("概要カテゴリーの設定", null, null, "食費", "日用雑貨", "交通費", "娯楽費", "医療費", "通信費", "水道_光熱費", "その他_支出");
                        switch (scategory)
                        {
                            case "食費":
                                dcategory = await _pageDialogService.DisplayActionSheetAsync("詳細カテゴリーの設定", null, null, "食料品", "朝食", "昼食", "夕食", "その他_食費");
                                break;
                            case "日用雑貨":
                                dcategory = await _pageDialogService.DisplayActionSheetAsync("詳細カテゴリーの設定", null, null, "消耗品", "子供関連", "ペット関連", "その他_日用雑貨");
                                break;
                            case "交通費":
                                dcategory = await _pageDialogService.DisplayActionSheetAsync("詳細カテゴリーの設定", null, null, "電車", "タクシー", "バス", "飛行機", "その他_交通費");
                                break;
                            case "娯楽費":
                                dcategory = await _pageDialogService.DisplayActionSheetAsync("詳細カテゴリーの設定", null, null, "レジャー", "イベント", "映画", "音楽", "漫画", "書籍", "ゲーム", "その他_娯楽費");
                                break;
                            case "医療費":
                                dcategory = await _pageDialogService.DisplayActionSheetAsync("詳細カテゴリーの設定", null, null, "病院代", "薬代", "生命保険", "医療保険", "その他_医療費");
                                break;
                            case "通信費":
                                dcategory = await _pageDialogService.DisplayActionSheetAsync("詳細カテゴリーの設定", null, null, "携帯電話料金", "固定電話料金", "インターネット関連", "テレビ受信料", "宅配便", "切手_はがき", "その他_通信費");
                                break;
                            case "水道_光熱費":
                                dcategory = await _pageDialogService.DisplayActionSheetAsync("詳細カテゴリーの設定", null, null, "水道料金", "電気料金", "ガス料金", "その他_水道_光熱費");
                                break;
                            case "その他_支出":
                                dcategory = await _pageDialogService.DisplayActionSheetAsync("詳細カテゴリーの設定", null, null, "仕送り", "お小遣い", "使途不明金", "立替金", "その他_支出");
                                break;
                            default:
                                dcategory = "食料品"; //とりあえず代入
                                break;
                        }
                        storagetype = await _pageDialogService.DisplayActionSheetAsync("出金元の設定", null, null, "財布", "貯金", "銀行", "クレジットカード", "その他");

                        var Scategory = (SCategorys)Enum.Parse(typeof(SCategorys), scategory);
                        var Dcategory = (DCategorys)Enum.Parse(typeof(DCategorys), dcategory);
                        var Storagetype = (StorageTypes)Enum.Parse(typeof(StorageTypes), storagetype);
                        await _storageService.BoughtWishItem(wishitem, hasID, true, Scategory, Dcategory, Storagetype);
                    }
                    /* 家計簿に登録しない場合 */
                    else
                    {
                        /* とりあえず代入 */
                        var Scategory = SCategorys.start_of_支出;
                        var Dcategory = DCategorys.start_of_その他_支出;
                        var Storagetype = StorageTypes.start_of_Stype;
                        await _storageService.BoughtWishItem(wishitem, hasID, false, Scategory, Dcategory, Storagetype);
                    }
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
