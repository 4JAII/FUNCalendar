using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using OxyPlot;
using OxyPlot.Xamarin.Forms;
using OxyPlot.Series;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using FUNCalendar.Models;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Xamarin.Forms;
using Prism.Navigation;
using Prism.Services;
using System.Text.RegularExpressions;
using System.ComponentModel.DataAnnotations;
using FUNCalendar.Services;

namespace FUNCalendar.ViewModels
{
    public class HouseholdAccountsEditBalancePageViewModel : BindableBase, INavigationAware, IDisposable
    {
        private IHouseholdAccounts _householdAccounts;
        private IStorageService _storageService;
        private INavigationService _navigationService;
        private IPageDialogService _pageDialogService;

        private CompositeDisposable disposable { get; } = new CompositeDisposable();

        public static readonly string EditKey = "EditKey";

        public HouseholdAccountsNavigationItem NavigatedItem { get; set; }

        public DateTime CurrentDate { get; private set; }
        public Range CurrentRange { get; private set; }
        public StorageTypes CurrentStoragetype;
        public VMHouseholdAccountsBalanceItem CurrentBalanceItem { get; private set; }
        public int PreviousPrice { get; set; }

        private string _title;
        public string Title
        {
            get { return this._title; }
            set { this.SetProperty(ref this._title, value); }
        }
        private string _storagetypeName;
        public string StoragetypeName
        {
            get { return this._storagetypeName; }
            set { this.SetProperty(ref this._storagetypeName, value); }
        }
        private ImageSource _image;
        public ImageSource Image
        {
            get { return this._image; }
            set { this.SetProperty(ref this._image, value); }
        }
        public int ID { get; private set; }
        [Required(ErrorMessage = "金額を入力してください")]
        [RegularExpression("-?[0-9]+")]
        public ReactiveProperty<string> Price { get; private set; } = new ReactiveProperty<string>();

        public ReactiveProperty<bool> CanRegister { get; private set; }
        public AsyncReactiveCommand RegisterHouseholdAccountsBalanceCommand { get; private set; }
        public AsyncReactiveCommand CancelCommand { get; private set; }

        public ReactiveProperty<Color> ErrorColor { get; private set; } = new ReactiveProperty<Color>();

        public HouseholdAccountsEditBalancePageViewModel(IHouseholdAccounts householdAccounts, IStorageService storageService, INavigationService navigationService, IPageDialogService pageDialogService)
        {
            this._householdAccounts = householdAccounts;
            this._storageService = storageService;
            this._navigationService = navigationService;
            this._pageDialogService = pageDialogService;

            /* 属性を有効化 */
            Price.SetValidateAttribute(() => this.Price);



            CanRegister = new[] {
                this.Price.ObserveHasErrors
            }.CombineLatestValuesAreAllFalse().ToReactiveProperty<bool>();
            CanRegister.Subscribe(x =>
            {
                ErrorColor.Value = x ? Color.SkyBlue : Color.Gray;
            });

            RegisterHouseholdAccountsBalanceCommand = CanRegister.ToAsyncReactiveCommand();
            RegisterHouseholdAccountsBalanceCommand.Subscribe(async () =>
            {
                var navigationitem = new HouseholdAccountsNavigationItem(CurrentDate, CurrentRange);
                var navigationparameter = new NavigationParameters()
                {
                    {HouseholdAccountsBalancePageViewModel.InputKey, navigationitem }
                };


                var EnteredPrice = int.Parse(Price.Value);
                var difference = EnteredPrice - PreviousPrice;

                var name = string.Format("残高・{0}の金額の調整", CurrentStoragetype);
                var date = DateTime.Today;

                /* マイナスだった場合 */
                if ((difference) < 0)
                {
                    var price = -difference;
                    var scategory = SCategorys.その他_支出;
                    var dcategory = DCategorys.その他_支出;
                    var isoutgoing = true;
                    var item = new HouseholdAccountsItem(name, price, date, dcategory, scategory, CurrentStoragetype, isoutgoing);
                    await _storageService.AddItem(item);
                }
                /* プラスだった場合 */
                else if (difference > 0)
                {
                    var price = difference;
                    var scategory = SCategorys.その他_収入;
                    var dcategory = DCategorys.その他_収入;
                    var isoutgoing = false;
                    var item = new HouseholdAccountsItem(name, price, date, dcategory, scategory, CurrentStoragetype, isoutgoing);
                    await _storageService.AddItem(item);
                }

                await _navigationService.NavigateAsync("/RootPage/NavigationPage/HouseholdAccountsBalancePage", navigationparameter);
            });

            CancelCommand = new AsyncReactiveCommand();
            CancelCommand.Subscribe(async () =>
            {
                var navigationitem = new HouseholdAccountsNavigationItem(CurrentDate, CurrentRange);
                var navigationparameter = new NavigationParameters()
                {
                    {HouseholdAccountsBalancePageViewModel.InputKey, navigationitem }
                };
                var result = await _pageDialogService.DisplayAlertAsync("確認", "入力をキャンセルし画面を変更します。よろしいですか？", "はい", "いいえ");
                if (result) await _navigationService.NavigateAsync("/RootPage/NavigationPage/HouseholdAccountsBalancePage", navigationparameter);
            });

        }

        public void OnNavigatedFrom(NavigationParameters parameters)
        {

        }
        public void OnNavigatedTo(NavigationParameters parameters)
        {
            if (parameters.ContainsKey(EditKey))
            {
                NavigatedItem = (HouseholdAccountsNavigationItem)parameters[EditKey];
                this.CurrentDate = NavigatedItem.CurrentDate;
                this.CurrentRange = NavigatedItem.CurrentRange;
                this.CurrentStoragetype = NavigatedItem.CurrentStoragetype;
                this.Title = string.Format("{0}の残高の編集", CurrentStoragetype);
                this.StoragetypeName = Enum.GetName(typeof(StorageTypes), CurrentStoragetype);
                this.Image= (CurrentStoragetype == StorageTypes.財布) ? ImageSource.FromFile("icon_wallet.png") :
                (CurrentStoragetype == StorageTypes.貯金) ? ImageSource.FromFile("icon_savings.png") :
                (CurrentStoragetype == StorageTypes.銀行) ? ImageSource.FromFile("icon_bank.png") :
                (CurrentStoragetype == StorageTypes.クレジットカード) ? ImageSource.FromFile("icon_credit.png") :
                (CurrentStoragetype == StorageTypes.SUICA) ? ImageSource.FromFile("icon_train.png") :
                (CurrentStoragetype == StorageTypes.その他) ? ImageSource.FromFile("icon_other.png") : ImageSource.FromFile("");
                Regex re = new Regex("円");
                Price.Value = re.Replace(NavigatedItem.Price, "");
                PreviousPrice = int.Parse(re.Replace(NavigatedItem.Price,""));
            }

        }
        public void OnNavigatingTo(NavigationParameters parameters)
        {

        }

        /* 購読解除 */
        public void Dispose()
        {
            disposable.Dispose();
        }

    }
}
