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
    public class HouseholdAccountsDCHistoryPageViewModel : BindableBase, INavigationAware, IDisposable
    {
        public ReactiveCommand BackPageCommand { get; private set; }

        private IHouseholdAccounts _householdaccounts;
        private INavigationService _navigationservice;

        public static readonly string InputKey = "InputKey";

        /* タイトル */
        private string _dCategoryTitle;
        public string DCategoryTitle
        {
            get { return this._dCategoryTitle; }
            set { this.SetProperty(ref this._dCategoryTitle, value); }
        }

        /* 履歴 */
        public ReadOnlyReactiveCollection<VMHouseholdAccountsItem> DisplayHistoryCollection { get; private set; }

        public HouseholdAccountsNavigationItem NavigatedItem { get; set; }
        private BalanceTypes CurrentBalanceType { get; set; }
        private SCategorys _currentSCategory;
        public SCategorys CurrentSCategory
        {
            get { return this._currentSCategory; }
            set { this.SetProperty(ref this._currentSCategory, value); }
        }
        private DCategorys _currentDCategory;
        public DCategorys CurrentDCategory
        {
            get { return this._currentDCategory; }
            set { this.SetProperty(ref this._currentDCategory, value); }
        }

        public HouseholdAccountsRangeItem[] RangeNames { get; private set; }

        public ReactiveProperty<DateTime> SelectedDate { get; private set; }
        public ReactiveProperty<HouseholdAccountsRangeItem> SelectedRange { get; private set; }

        public ReactiveCommand ResistCommand { get; private set; }

        private CompositeDisposable disposable { get; } = new CompositeDisposable();

        public HouseholdAccountsDCHistoryPageViewModel(IHouseholdAccounts householdaccounts, INavigationService navigationService)
        {
            this._householdaccounts = householdaccounts;
            this._navigationservice = navigationService;
            this.ResistCommand = new ReactiveCommand();

            DisplayHistoryCollection = _householdaccounts.DisplayHouseholdaccountList.ToReadOnlyReactiveCollection(x => new VMHouseholdAccountsItem(x)).AddTo(disposable);

            /* インスタンス化 */
            this.BackPageCommand = new ReactiveCommand();
            SelectedRange = new ReactiveProperty<HouseholdAccountsRangeItem>();
            SelectedDate = new ReactiveProperty<DateTime>();



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

            /* 概要カテゴリーの統計ページに遷移 */
            BackPageCommand.Subscribe(_ =>
            {
                var navigationitem = new HouseholdAccountsNavigationItem(CurrentBalanceType, CurrentSCategory, SelectedDate.Value, SelectedRange.Value.RangeData);
                var navigationparameter = new NavigationParameters()
                {
                    {HouseholdAccountsStatisticsPageViewModel.InputKey, navigationitem }
                };
                _navigationservice.NavigateAsync("/RootPage/NavigationPage/HouseholdAccountsSCStatisticsPage", navigationparameter);
            }).AddTo(disposable);


            /* デバッグ用 アイテム追加ボタンが押された時の処理 */
            ResistCommand.Subscribe(_ =>
            {
                DateTime temp = new DateTime(2017, 12, 16);
                _householdaccounts.AddHouseholdAccountsItem("test1", 100, temp, DCategorys.朝食, SCategorys.食費, StorageTypes.財布, true);
                _householdaccounts.AddHouseholdAccountsItem("test2", 300, DateTime.Today, DCategorys.消耗品, SCategorys.日用雑貨, StorageTypes.財布, true);
                _householdaccounts.AddHouseholdAccountsItem("test3", 500, DateTime.Today, DCategorys.子供関連, SCategorys.日用雑貨, StorageTypes.財布, true);
                _householdaccounts.AddHouseholdAccountsItem("test4", 500, DateTime.Today, DCategorys.受取利息, SCategorys.投資収入, StorageTypes.財布, false);
                _householdaccounts.AddHouseholdAccountsItem("test4", 2000, temp, DCategorys.その他_収入, SCategorys.その他_収入, StorageTypes.財布, false);
                _householdaccounts.SetDCategoryHistory(SelectedRange.Value.RangeData, SelectedDate.Value, CurrentDCategory);
            }).AddTo(disposable);
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
                this.CurrentBalanceType = NavigatedItem.CurrentBalanceType;
                this.SelectedRange.Value = (NavigatedItem.CurrentRange == Range.Day) ? RangeNames[0] :
                    (NavigatedItem.CurrentRange == Range.Month) ? RangeNames[1] :
                    (NavigatedItem.CurrentRange == Range.Year) ? RangeNames[2] : null;
                this.CurrentSCategory = NavigatedItem.CurrentSCategory;
                this.CurrentDCategory = NavigatedItem.CurrentDCategory;
                this.DCategoryTitle = String.Format("家計簿・{0}・{1}", NavigatedItem.CurrentSCategory, NavigatedItem.CurrentDCategory);
                _householdaccounts.SetDCategoryHistory(SelectedRange.Value.RangeData, SelectedDate.Value, CurrentDCategory);

                /* 日付が変更された時の処理 */
                SelectedDate.Subscribe(_ =>
                {
                    if (_ != null)
                    {
                        _householdaccounts.SetDCategoryHistory(SelectedRange.Value.RangeData, SelectedDate.Value, CurrentDCategory);
                    }
                })
                .AddTo(disposable);

                /* レンジが変更された時の処理 */
                SelectedRange.Subscribe(_ =>
                {
                    if (_ != null)
                    {
                        _householdaccounts.SetDCategoryHistory(SelectedRange.Value.RangeData, SelectedDate.Value, CurrentDCategory);
                    }
                })
                .AddTo(disposable);

            }
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
