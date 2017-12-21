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

namespace FUNCalendar.ViewModels
{
    public class WishListRegisterPageViewModel : BindableBase, INavigationAware
    {
        private IWishList _wishList;
        private INavigationService _navigationService;
        private IPageDialogService _pageDialogService;

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

        /* データベース用 */
        private LocalStorage localStorage;

        /* 廃棄 */
        private CompositeDisposable disposable { get; } = new CompositeDisposable();

        public WishListRegisterPageViewModel(IWishList wishList, INavigationService navigationService, IPageDialogService pageDialogService)
        {
            localStorage = new LocalStorage();

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
                    await localStorage.EditItem(wishItem);
                    if (NeedsAdd)
                    {
                        /* wishItemをToDoItemに変換してadd処理 
                         * localStorage.AddItem(todoItem);*/
                    }
                    _wishList.EditWishItem(_wishList.DisplayWishItem, wishItem);

                }
                else
                {
                    if (NeedsAdd)
                    {
                        /* wishItemをToDoItemに変換してadd処理 
                         * localStorage.AddItem(todoItem);*/
                    }
                    var wishItem = new WishItem { Name = this.Name.Value, Price = int.Parse(this.Price.Value), Date = Date.Value, IsBought = false };
                    await localStorage.AddItem(new WishItem(this.Name.Value, int.Parse(this.Price.Value), Date.Value, false,/*ここにID*/-1));
                    wishItem.ID = localStorage.LastAddedWishItemID;
                    _wishList.AddWishItem(wishItem);
                }
                await _navigationService.NavigateAsync($"/RootPage/NavigationPage/WishListPage");
            });

            /* 登録をキャンセルして遷移(確認もあるよ)  */
            CancelCommand = new AsyncReactiveCommand();
            CancelCommand.Subscribe(async () =>
            {
                var result = await _pageDialogService.DisplayAlertAsync("確認", "入力をキャンセルし画面を変更します。よろしいですか？", "はい", "いいえ");
                if (result) await _navigationService.NavigateAsync($"/RootPage/NavigationPage/WishListPage");
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
