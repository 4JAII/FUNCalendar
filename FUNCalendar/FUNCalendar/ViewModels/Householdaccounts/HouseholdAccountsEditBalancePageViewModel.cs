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
    public class HouseholdAccountsEditBalancePageViewModel : BindableBase,INavigationAware, IDisposable
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
        private string _currentstoragetype;
        public string CurrentStoragetype
        {
            get { return this._currentstoragetype; }
            set { this.SetProperty(ref this._currentstoragetype, value); }
        }
        public VMHouseholdAccountsBalanceItem CurrentBalanceItem { get; private set; }

        public int ID { get; private set; }
        [Required (ErrorMessage ="金額を入力してください")]
        [RegularExpression("[0-9]+")]
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



            CanRegister =new[] {
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


                var DeleteBItem = VMHouseholdAccountsBalanceItem.ToHouseholdAccountsBalanceItem(CurrentBalanceItem);
                var AddBItem = new HouseholdAccountsBalanceItem() { ID = DeleteBItem.ID, Storagetype = DeleteBItem.Storagetype, Price = int.Parse(Price.Value) };
                //_householdAccounts.EditHouseholdAccountsBalanceItem(item, int.Parse(Price.Value), false, true);
                _householdAccounts.EditHouseholdAccountsBalanceItem(DeleteBItem,AddBItem);

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

                CurrentBalanceItem = new VMHouseholdAccountsBalanceItem(_householdAccounts.SelectedBalanceItem);
                Regex re = new Regex("[^0-9]");
                ID = CurrentBalanceItem.ID;
                Price.Value = re.Replace(CurrentBalanceItem.Price, "");
                CurrentStoragetype = CurrentBalanceItem.StorageType;
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
