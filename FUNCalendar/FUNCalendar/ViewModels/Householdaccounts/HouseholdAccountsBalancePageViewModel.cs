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
using FUNCalendar.Services;



namespace FUNCalendar.ViewModels
{
    public class HouseholdAccountsBalancePageViewModel : BindableBase, INavigationAware, IDisposable
    {
        private IHouseholdAccounts _householdaccounts;
        private INavigationService _navigationService;

        public static readonly string InputKey = "InputKey";

        public ReadOnlyReactiveCollection<VMHouseholdAccountsBalanceItem> DisplayBalances { get; private set; }
        public ReactiveProperty<string> DisplayTotalBalance { get; private set; }

        public HouseholdAccountsNavigationItem NavigatedItem { get; set; }

        /* 現在選択されてる各データを保持 */
        public DateTime CurrentDate { get; private set; }
        public Range CurrentRange { get; private set; }

        /* 履歴画面移行用 */
        public AsyncReactiveCommand HistoryCommand { get; private set; }

        /* 統計画面移行用 */
        public AsyncReactiveCommand StatisticsCommand { get; private set; }


        /* アイテム追加コマンド */
        public AsyncReactiveCommand ResistCommand { get; private set; }

        /* 編集コマンド */
        public ReactiveCommand EditCommand { get; private set; }

        /* 購読解除 */
        private CompositeDisposable disposable { get; } = new CompositeDisposable();

        public HouseholdAccountsBalancePageViewModel(IHouseholdAccounts ihouseholdaccounts, INavigationService navigationService)
        {
            this.ResistCommand = new AsyncReactiveCommand();

            this._householdaccounts = ihouseholdaccounts;
            this._navigationService = navigationService;

            this.DisplayBalances = _householdaccounts.Balances.ToReadOnlyReactiveCollection(x => new VMHouseholdAccountsBalanceItem(x)).AddTo(disposable);
            this.DisplayTotalBalance = _householdaccounts.ObserveProperty(h => h.TotalBalance).ToReactiveProperty().AddTo(disposable);

            /* インスタンス化 */
            HistoryCommand = new AsyncReactiveCommand();
            StatisticsCommand = new AsyncReactiveCommand();
            EditCommand = new ReactiveCommand();



            /* 履歴ボタンが押されたときの処理 */
            HistoryCommand.Subscribe(async _ =>
            {
                var navigationitem = new HouseholdAccountsNavigationItem(CurrentDate, CurrentRange);
                var navigationparameter = new NavigationParameters()
                {
                    {HouseholdAccountsBalancePageViewModel.InputKey, navigationitem }
                };
                await _navigationService.NavigateAsync("/RootPage/NavigationPage/HouseholdAccountsHistoryPage", navigationparameter);
            }).AddTo(disposable);

            /* 統計ボタンが押されたときの処理 */
            StatisticsCommand.Subscribe(async _ =>
            {
                var navigationitem = new HouseholdAccountsNavigationItem(CurrentDate, CurrentRange);
                var navigationparameter = new NavigationParameters()
                {
                    {HouseholdAccountsBalancePageViewModel.InputKey, navigationitem }
                };
                await _navigationService.NavigateAsync("/RootPage/NavigationPage/HouseholdAccountsStatisticsPage", navigationparameter);
            }).AddTo(disposable);



            /* アイテム追加ボタンが押された時の処理 */
            ResistCommand.Subscribe(async _ =>
            {
                var navigationitem = new HouseholdAccountsNavigationItem(CurrentDate, CurrentRange);
                var navigationparameter = new NavigationParameters()
                {
                    {HouseholdAccountsRegisterPageViewModel.InputKey, navigationitem }
                };
                navigationparameter.Add("BackPage", PageName.HouseholdAccountsBalancePage);
                await _navigationService.NavigateAsync("/RootPage/NavigationPage/HouseholdAccountsRegisterPage", navigationparameter);
            }).AddTo(disposable);

            /* 残高の編集ボタンが押されたときの処理 */
            EditCommand.Subscribe(async (obj) =>
            {
                var storagetype = (obj as VMHouseholdAccountsBalanceItem).StorageType;
                var price = (obj as VMHouseholdAccountsBalanceItem).Price;

                var Storagetype = (StorageTypes)Enum.Parse(typeof(StorageTypes), storagetype);
                var navigationitem = new HouseholdAccountsNavigationItem(Storagetype, price, CurrentDate, CurrentRange);
                var navigationparameter = new NavigationParameters()
                {
                    {HouseholdAccountsEditBalancePageViewModel.EditKey, navigationitem }
                };

                await _navigationService.NavigateAsync("/RootPage/NavigationPage/HouseholdAccountsEditBalancePage", navigationparameter);

            }).AddTo(disposable);
        }

        public void Dispose()
        {
            disposable.Dispose();
        }

        public void OnNavigatedFrom(NavigationParameters parameters)
        {
            this.Dispose();
        }

        public void OnNavigatedTo(NavigationParameters parameters)
        {
            if (parameters.ContainsKey(InputKey))
            {
                NavigatedItem = (HouseholdAccountsNavigationItem)parameters[InputKey];
                this.CurrentDate = NavigatedItem.CurrentDate;
                this.CurrentRange = NavigatedItem.CurrentRange;
                _householdaccounts.SetBalance();
            }
        }

        public void OnNavigatingTo(NavigationParameters parameters)
        {
        }
    }
}
