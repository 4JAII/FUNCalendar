using FUNCalendar.Models;
using FUNCalendar.Services;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.Services;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using System;
using System.ComponentModel.DataAnnotations;
using System.Reactive.Disposables;
using System.Text.RegularExpressions;
using Xamarin.Forms;

namespace FUNCalendar.ViewModels
{
    public class WishListRegisterPageViewModel : BindableBase, INavigationAware
    {
        private IWishList _wishList;
        private INavigationService _navigationService;
        private IPageDialogService _pageDialogService;
        private IStorageService _storageService;

        /* WishItem登録用 */
        public int ID { get; private set; } = -1;
        [Required(ErrorMessage = "商品名がありません"), StringLength(32)]
        public ReactiveProperty<string> Name { get; private set; } = new ReactiveProperty<string>();
        [Required(ErrorMessage = "値段がありません"), StringLength(9)]
        [RegularExpression("[0-9]+")]
        public ReactiveProperty<string> Price { get; private set; } = new ReactiveProperty<string>();
        [Required(ErrorMessage = "日付がありません")]
        public ReactiveProperty<DateTime> Date { get; private set; } = new ReactiveProperty<DateTime>();
        public bool NeedsAdd { get; set; } = false;
        private string isBought;
        private int todoID;

        /* 登録・キャンセルするときの処理用 */
        public ReactiveProperty<bool> CanRegister { get; private set; }
        public AsyncReactiveCommand RegisterWishItemCommand { get; private set; }
        public AsyncReactiveCommand CancelCommand { get; private set; }

        /* エラー時の色 */
        public ReactiveProperty<Color> ErrorColor { get; private set; } = new ReactiveProperty<Color>();

        /* ページ遷移用 */
        private string backPage;

        /* 廃棄 */
        private CompositeDisposable disposable { get; } = new CompositeDisposable();

        public WishListRegisterPageViewModel(IWishList wishList, IStorageService storage, INavigationService navigationService, IPageDialogService pageDialogService)
        {
            this._storageService = storage;

            /* コンストラクタインジェクションされたインスタンスを保持 */
            this._wishList = wishList;
            this._navigationService = navigationService;
            this._pageDialogService = pageDialogService;
            /* 属性を有効化 */
            Name.SetValidateAttribute(() => this.Name);
            Price.SetValidateAttribute(() => this.Price);
            Date.SetValidateAttribute(() => this.Date);

            Date.Value = DateTime.Now;/* とりあえず現在の日付に合わせておく */

            /* 全てにエラーなしなら登録できるようにする */
            CanRegister = new[]
            {
                this.Name.ObserveHasErrors,
                this.Price.ObserveHasErrors,
                this.Date.ObserveHasErrors,
            }
            .CombineLatestValuesAreAllFalse()
            .ToReactiveProperty<bool>();

            /* 登録して遷移 */
            RegisterWishItemCommand = CanRegister.ToAsyncReactiveCommand();
            RegisterWishItemCommand.Subscribe(async () =>
            {
                if (ID != -1)
                {
                    var vmWishItem = new VMWishItem(ID, Name.Value, Price.Value, Date.Value, isBought, todoID);
                    var wishItem = VMWishItem.ToWishItem(vmWishItem);
                    if (!await _pageDialogService.DisplayAlertAsync("確認", "もしToDo連携済みのものの連携を切る場合,連携したToDoは削除されます", "はい", "いいえ"))
                        return;
                    string priority = "0";
                    if (NeedsAdd)
                        priority = await _pageDialogService.DisplayActionSheetAsync("優先度を選んでください", null, null, "1", "2", "3", "4", "5");

                    await _storageService.EditItem(_wishList.DisplayWishItem, wishItem, NeedsAdd, int.Parse(priority));

                }
                else
                {
                    var wishItem = new WishItem { Name = this.Name.Value, Price = int.Parse(this.Price.Value), Date = Date.Value, IsBought = false };
                    string priority = "0";
                    if (NeedsAdd)
                        priority = await _pageDialogService.DisplayActionSheetAsync("優先度を選んでください", null, null, "1", "2", "3", "4", "5");
                    await _storageService.AddItem(wishItem, NeedsAdd, int.Parse(priority));
                }
                await _navigationService.NavigateAsync(backPage);
            });

            /* 登録をキャンセルして遷移(確認もあるよ)  */
            CancelCommand = new AsyncReactiveCommand();
            CancelCommand.Subscribe(async () =>
            {
                var result = await _pageDialogService.DisplayAlertAsync("確認", "入力をキャンセルし画面を変更します。よろしいですか？", "はい", "いいえ");
                if (result) await _navigationService.NavigateAsync(backPage);
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
            backPage = parameters["BackPage"] as string;
            /* 編集目的で遷移してきたならセット */
            if (parameters["FromCalendar"] as string == "T")
            {
                Date.Value = Convert.ToDateTime(parameters["DateData"]);
            }
            if (parameters["CanEdit"] as string != "T") return;
            VMWishItem vmWishItem = new VMWishItem(_wishList.DisplayWishItem);
            Regex re = new Regex(@"[^0-9]");
            ID = vmWishItem.ID;
            Name.Value = vmWishItem.Name;
            Price.Value = re.Replace(vmWishItem.Price, "");
            Date.Value = _wishList.DisplayWishItem.Date;
            isBought = vmWishItem.IsBought;
            todoID = vmWishItem.ToDoID;
        }

        public void OnNavigatingTo(NavigationParameters parameters)
        {

        }
    }
}
