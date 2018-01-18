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



namespace FUNCalendar.ViewModels
{
    public class HouseholdAccountsBalancePageViewModel : BindableBase, INavigationAware, IDisposable
    {
        private IHouseholdAccounts _householdaccounts;
        private INavigationService _navigationservice;

        public static readonly string InputKey = "InputKey";


        public ReadOnlyReactiveCollection<VMHouseholdAccountsBalanceItem> DisplayBalances { get; private set; }
        public ReactiveProperty<string> DisplayTotalBalance { get; private set; }

        public HouseholdAccountsNavigationItem NavigatedItem { get; set; }

        public HouseholdAccountsRangeItem[] RangeNames { get; private set; }

        public ReactiveProperty<DateTime> SelectedDate { get; private set; }
        public ReactiveProperty<HouseholdAccountsRangeItem> SelectedRange { get; private set; }

        /* 履歴画面移行用 */
        public ReactiveCommand HistoryCommand { get; private set; }

        /* 統計画面移行用 */
        public ReactiveCommand StatisticsCommand { get; private set; }


        /* デバッグ用 */
        public ReactiveCommand ResistCommand { get; private set; }

        private CompositeDisposable disposable { get; } = new CompositeDisposable();

        public HouseholdAccountsBalancePageViewModel(IHouseholdAccounts ihouseholdaccounts, INavigationService navigationService)
        {
            /* デバッグ用 */
            this.ResistCommand = new ReactiveCommand();

            _householdaccounts = ihouseholdaccounts;
            _navigationservice = navigationService;
            this.DisplayBalances = _householdaccounts.Balances.ToReadOnlyReactiveCollection(x => new VMHouseholdAccountsBalanceItem(x)).AddTo(disposable);
            this.DisplayTotalBalance = _householdaccounts.ObserveProperty(h => h.TotalBalance).ToReactiveProperty().AddTo(disposable);

            /* インスタンス化 */
            SelectedRange = new ReactiveProperty<HouseholdAccountsRangeItem>();
            SelectedDate = new ReactiveProperty<DateTime>();
            HistoryCommand = new ReactiveCommand();
            StatisticsCommand = new ReactiveCommand();



            /* ピッカー用のアイテムの作成 */
            RangeNames = new[]
            {
                new HouseholdAccountsRangeItem
                {
                    RangeName = "統計:日単位",
                    RangeData = Range.Day
                },
                new HouseholdAccountsRangeItem
                {
                    RangeName = "統計:月単位" ,
                    RangeData = Range.Month
                },
                new HouseholdAccountsRangeItem
                {
                    RangeName = "統計:年単位",
                    RangeData = Range.Year
                }
            };

            /* 履歴ボタンが押されたときの処理 */
            HistoryCommand.Subscribe(_ =>
            {
                var navigationitem = new HouseholdAccountsNavigationItem(SelectedDate.Value, SelectedRange.Value.RangeData);
                var navigationparameter = new NavigationParameters()
                {
                    {HouseholdAccountsBalancePageViewModel.InputKey, navigationitem }
                };
                _navigationservice.NavigateAsync("/RootPage/NavigationPage/HouseholdAccountsHistoryPage", navigationparameter);
            }).AddTo(disposable);

            /* 統計ボタンが押されたときの処理 */
            StatisticsCommand.Subscribe(_ =>
            {
                var navigationitem = new HouseholdAccountsNavigationItem(SelectedDate.Value, SelectedRange.Value.RangeData);
                var navigationparameter = new NavigationParameters()
                {
                    {HouseholdAccountsBalancePageViewModel.InputKey, navigationitem }
                };
                _navigationservice.NavigateAsync("/RootPage/NavigationPage/HouseholdAccountsStatisticsPage", navigationparameter);
            }).AddTo(disposable);



            /* デバッグ用 アイテム追加ボタンが押された時の処理 */
            ResistCommand.Subscribe(_ =>
            {
                DateTime temp = new DateTime(2017, 12, 16);
                _householdaccounts.AddHouseholdAccountsItem("test1", 100, temp, DCategorys.朝食, SCategorys.食費, StorageTypes.財布, true);
                _householdaccounts.SetBalance();
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
                this.SelectedDate.Value = NavigatedItem.CurrentDate;
                this.SelectedRange.Value = (NavigatedItem.CurrentRange == Range.Day) ? RangeNames[0] :
                    (NavigatedItem.CurrentRange == Range.Month) ? RangeNames[1] :
                    (NavigatedItem.CurrentRange == Range.Year) ? RangeNames[2] : null;
                _householdaccounts.SetBalance();
            }
        }

        public void OnNavigatingTo(NavigationParameters parameters)
        {
        }
    }
}
