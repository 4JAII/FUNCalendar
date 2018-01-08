﻿using Prism.Commands;
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
    public class HouseHoldAccountsSCStatisticsPageViewModel : BindableBase, INavigationAware, IDisposable
    {
        public ReactiveCommand BackPageCommand { get; private set; }

        private IHouseHoldAccounts _householdaccounts;
        private INavigationService _inavigationservice;

        /* 正しい遷移か確認するためのkey */
        public static readonly string InputKey = "InputKey";

        /* 統計 */
        public ReadOnlyReactiveCollection<VMHouseholdaccountDcStatisticsItem> DisplayScategoryItems { get; private set; }
        public ReactiveProperty<string> DisplayScategoryTotal { get; private set; }

        /* 遷移されたときの格納用変数 */
        public HouseholdaccountNavigationItem NavigatedItem { get; set; }
        private BalanceTypes CurrentBalanceType { get; set; }
        private SCategorys _currentSCategory;
        public SCategorys CurrentSCategory
        {
            get { return this._currentSCategory; }
            set { this.SetProperty(ref this._currentSCategory, value); }
        }
        private string _sCategoryTitle;
        public string SCategoryTitle
        {
            get { return this._sCategoryTitle; }
            set { this.SetProperty(ref this._sCategoryTitle, value); }
        }


        /* グラフ */
        public PlotModel _plotmodel { get; set; } = new PlotModel();
        public PlotModel DisplayPlotModel { get; private set; }
        public ReadOnlyReactiveCollection<HouseholdaccountPieSliceItem> Slices { get; private set; }
        public List<PieSlice> DisplaySlices { get; private set; }
        private PieSeries pieseries = new PieSeries()
        {
            StrokeThickness = 2.0,
            InsideLabelPosition = 0.5,
            AngleSpan = 360,
            StartAngle = 270,
            InnerDiameter = 0.8
        };
        public ReadOnlyReactiveCollection<VMHouseholdaccountLegendItem> DisplayLegend { get; set; }

        /* Picker用のアイテム */
        public HouseholdaccountRangeItem[] RangeNames { get; private set; }


        public ReactiveProperty<DateTime> SelectedDate { get; private set; }    /* 日付を格納 */
        public ReactiveProperty<HouseholdaccountRangeItem> SelectedRange { get; private set; }   /* 範囲を格納 */

        /* 詳細画面移行用 */
        public Command<VMHouseholdaccountDcStatisticsItem> ItemSelectedCommand { get; }


        /* デバッグ用 */
        public ReactiveCommand ResistCommand { get; private set; }

        /* 購読解除 */
        private CompositeDisposable disposable { get; } = new CompositeDisposable();


        public HouseHoldAccountsSCStatisticsPageViewModel(IHouseHoldAccounts ihouseholdaccounts, INavigationService navigationService)
        {
            /* デバッグ用 アイテム追加コマンド */
            this.ResistCommand = new ReactiveCommand();

            this._householdaccounts = ihouseholdaccounts;
            this._inavigationservice = navigationService;

            /* ReactiveProperty化(統計) */
            this.DisplayScategoryItems = _householdaccounts.ScategoryItems.ToReadOnlyReactiveCollection(x => new VMHouseholdaccountDcStatisticsItem(x)).AddTo(disposable);
            this.DisplayScategoryTotal = _householdaccounts.ObserveProperty(h => h.SCategoryTotal).ToReactiveProperty().AddTo(disposable);

            /* ReactiveProperty化(グラフ) */
            this.Slices = _householdaccounts.PieSlice.ToReadOnlyReactiveCollection().AddTo(disposable);

            /* RectiveProperty化(グラフ凡例) */
            this.DisplayLegend = _householdaccounts.Legend.ToReadOnlyReactiveCollection(x => new VMHouseholdaccountLegendItem(x)).AddTo(disposable);

            /* インスタンス化 */
            this.BackPageCommand = new ReactiveCommand();
            SelectedRange = new ReactiveProperty<HouseholdaccountRangeItem>();
            DisplaySlices = new List<PieSlice>();
            SelectedDate = new ReactiveProperty<DateTime>();



            /* ピッカー用のアイテムの作成 */
            RangeNames = new[]
            {
                new HouseholdaccountRangeItem
                {
                    RangeName = "統計:日単位",
                    R = Range.Day
                },
                new HouseholdaccountRangeItem
                {
                    RangeName = "統計:月単位" ,
                    R = Range.Month
                },
                new HouseholdaccountRangeItem
                {
                    RangeName = "統計:年単位",
                    R = Range.Year
                }
            };

            /* plotmodelにsliceを追加 */
            pieseries.Slices = DisplaySlices;
            this._plotmodel.Series.Add(pieseries);
            UpdatePie();



            /* グラフのデータが変更された時の処理 */
            Slices.CollectionChangedAsObservable().Subscribe(_ => { UpdatePie(); });

            /* メインページに遷移 */
            BackPageCommand.Subscribe(_ =>
            {
                var navigationitem = new HouseholdaccountNavigationItem(SelectedDate.Value, SelectedRange.Value.R);
                var navigationparameter = new NavigationParameters()
                {
                    {HouseHoldAccountsStatisticsPageViewModel.InputKey, navigationitem }
                };
                _inavigationservice.NavigateAsync("/RootPage/NavigationPage/HouseHoldAccountsStatisticsPage", navigationparameter);
            }).AddTo(disposable);

            /* リストのアイテムがクリックされた時の処理 */
            ItemSelectedCommand = new Command<VMHouseholdaccountDcStatisticsItem>(x =>
            {
                var currentbalancetype = (BalanceTypes)Enum.Parse(typeof(BalanceTypes), x.Balancetype);
                var currentscategory = (SCategorys)Enum.Parse(typeof(SCategorys), x.Scategory);
                var currentdcategory = (DCategorys)Enum.Parse(typeof(DCategorys), x.Dcategory);

                var navigationitem = new HouseholdaccountNavigationItem(currentbalancetype, currentscategory, currentdcategory, SelectedDate.Value, SelectedRange.Value.R);
                var navigationparameter = new NavigationParameters()
                {
                    {HouseholdaccountsDCHistoryPageViewModel.InputKey, navigationitem }
                };
                _inavigationservice.NavigateAsync("/RootPage/NavigationPage/HouseholdaccountsDCHistoryPage", navigationparameter);
            });

            /* デバッグ用 アイテム追加ボタンが押された時の処理 */
            ResistCommand.Subscribe(_ =>
            {
                DateTime temp = new DateTime(2017, 12, 16);
                _householdaccounts.AddHouseHoldAccountsItem("test1", 100, temp, DCategorys.朝食, SCategorys.食費, StorageTypes.財布, true);
                _householdaccounts.AddHouseHoldAccountsItem("test2", 300, DateTime.Today, DCategorys.消耗品, SCategorys.日用雑貨, StorageTypes.財布, true);
                _householdaccounts.AddHouseHoldAccountsItem("test3", 500, DateTime.Today, DCategorys.子供関連, SCategorys.日用雑貨, StorageTypes.財布, true);
                _householdaccounts.AddHouseHoldAccountsItem("test4", 500, DateTime.Today, DCategorys.受取利息, SCategorys.投資収入, StorageTypes.財布, false);
                _householdaccounts.AddHouseHoldAccountsItem("test4", 2000, temp, DCategorys.その他_収入, SCategorys.その他_収入, StorageTypes.財布, false);
                _householdaccounts.SetSCategoryStatics(SelectedRange.Value.R, CurrentBalanceType, SelectedDate.Value, CurrentSCategory);
                _householdaccounts.SetSCategoryStatisticsPie(SelectedRange.Value.R, SelectedDate.Value, CurrentSCategory);
            }).AddTo(disposable);

        }
        public void OnNavigatedFrom(NavigationParameters parameters)
        {
            Dispose();
        }

        public void OnNavigatedTo(NavigationParameters parameters)
        {

        }

        public void OnNavigatingTo(NavigationParameters parameters)
        {
            if (parameters.ContainsKey(InputKey))
            {
                NavigatedItem = (HouseholdaccountNavigationItem)parameters[InputKey];
                this.SelectedDate.Value = NavigatedItem.CurrentDate;
                this.CurrentBalanceType = NavigatedItem.CurrentBalanceType;
                this.SelectedRange.Value = (NavigatedItem.CurrentRange == Range.Day) ? RangeNames[0] :
                    (NavigatedItem.CurrentRange == Range.Month) ? RangeNames[1] :
                    (NavigatedItem.CurrentRange == Range.Year) ? RangeNames[2] : null;
                this.CurrentSCategory = NavigatedItem.CurrentSCategory;
                this.SCategoryTitle = String.Format("家計簿・{0}", NavigatedItem.CurrentSCategory);

                _householdaccounts.SetSCategoryStatics(SelectedRange.Value.R, CurrentBalanceType, SelectedDate.Value, CurrentSCategory);
                _householdaccounts.SetSCategoryStatisticsPie(SelectedRange.Value.R, SelectedDate.Value, CurrentSCategory);
                this.DisplayScategoryTotal = _householdaccounts.ObserveProperty(h => h.SCategoryTotal).ToReactiveProperty().AddTo(disposable);

                /* ReactiveProperty化(グラフ) */
                this.Slices = _householdaccounts.PieSlice.ToReadOnlyReactiveCollection().AddTo(disposable);

                /* 日付が変更された時の処理 */
                SelectedDate.Subscribe(_ =>
                {
                    if (_ != null)
                    {
                        _householdaccounts.SetSCategoryStatics(SelectedRange.Value.R, CurrentBalanceType, SelectedDate.Value, CurrentSCategory);
                        _householdaccounts.SetSCategoryStatisticsPie(SelectedRange.Value.R, SelectedDate.Value, CurrentSCategory);
                    }
                })
                .AddTo(disposable);

                /* レンジが変更された時の処理 */
                SelectedRange.Subscribe(_ =>
                {
                    if (_ != null)
                    {
                        _householdaccounts.SetSCategoryStatics(SelectedRange.Value.R, CurrentBalanceType, SelectedDate.Value, CurrentSCategory);
                        _householdaccounts.SetSCategoryStatisticsPie(SelectedRange.Value.R, SelectedDate.Value, CurrentSCategory);
                    }
                })
                .AddTo(disposable);

            }
        }

        /* グラフの更新 */
        private void UpdatePie()
        {
            DisplaySlices.Clear();
            DisplaySlices.AddRange(Slices.Where(x => x.Price > 0).Select(x => new PieSlice(null, x.Price) { Fill = OxyColor.Parse(x.ColorPath) }));
            _plotmodel.Title = String.Format("{0}:{1}", CurrentSCategory, DisplayScategoryTotal.Value);
            DisplayPlotModel = _plotmodel;
            DisplayPlotModel.InvalidatePlot(true);
        }

        /* 購読解除 */
        public void Dispose()
        {
            disposable.Dispose();
        }
    }
}
