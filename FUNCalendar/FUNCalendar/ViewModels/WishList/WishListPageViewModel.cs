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

        /* 購入済みコマンド */
        public AsyncReactiveCommand BoughtCommand { get; private set; } = new AsyncReactiveCommand();

        public WishListPageViewModel(IWishList wishList, IStorageService storageService, INavigationService navigationService, IPageDialogService pageDialogService)
        {
            this._wishList = wishList;
            this._storageService = storageService;
            this._pageDialogService = pageDialogService;
            this._navigationService = navigationService;
            OrderChangeCommand = new ReactiveCommand();
            NavigationRegisterPageCommand = new AsyncReactiveCommand();
            SelectedSortName = new ReactiveProperty<WishListSortName>();

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
                await _navigationService.NavigateAsync($"/NavigationPage/WishListRegisterPage?CanEdit=T");
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
            NavigationRegisterPageCommand.Subscribe(async () => await this._navigationService.NavigateAsync($"/NavigationPage/WishListRegisterPage"));
            /* 選ばれた並べ替え方法が変わったとき */
            SelectedSortName.Subscribe(_ => { if (_ != null) SelectedSortName.Value.Sort(); }).AddTo(disposable);
            /* 昇順降順が変わった時 */
            OrderChangeCommand.Subscribe(() =>
            {
                _wishList.IsAscending = !_wishList.IsAscending;/* MVVM違反？*/
                Order = _wishList.IsAscending ? "昇順" : "降順";
                SelectedSortName.Value.Sort();
            }).AddTo(disposable);

            /* 購入完了ボタンが押されたときの処理 */
            BoughtCommand.Subscribe(async (obj)=>
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
                    if((obj as VMWishItem).ToDoID != 0)
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

