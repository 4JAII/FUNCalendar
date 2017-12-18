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

namespace FUNCalendar.ViewModels
{
    public class HouseHoldAccountsPageViewModel : BindableBase,IDisposable
    {
        private IHouseHoldAccounts _householdaccouts;                           /* 家計簿のinterface */
        public ReactiveProperty<DateTime> SelectedDate { get; private set; }    /* 日付を格納 */
        public ReactiveProperty<HouseholdaccoutRange> SelectedRange { get; private set; }   /* 範囲を格納 */

        public ReactiveProperty<string> DisplayTotalIncome { get; private set; }            /* 収入の合計値を格納 */
        public ReactiveProperty<string> DisplayTotalOutgoing { get; private set; }          /* 支出の合計値を格納 */
        public ReactiveProperty<string> DisplayDifference { get; private set; }             /* 差分地を格納 */
        public ReadOnlyReactiveCollection<VMHouseholdaccountScStatisticsItem> DisplayTotalIncomeCollection { get; private set; }        /* 収入の概要カテゴリーの合計値を格納 */
        public ReadOnlyReactiveCollection<VMHouseholdaccountScStatisticsItem> DisplayTotalOutgoingCollection { get; private set; }      /* 支出の概要カテゴリーの合計値を格納 */


        /* デバッグ用 */
        public ReactiveCommand ResistCommand { get; private set; }

        /* Picker用のアイテム */
        public HouseholdaccoutRange[] RangeNames { get; private set; }


        private CompositeDisposable disposable { get; } = new CompositeDisposable();

        /* グラフ */
        public ReactiveCommand SelectOutgoing { get; private set; }
        public ReactiveCommand SelectIncome { get; private set; }
        public ReactiveCommand SelectDifference { get; private set; }
        private PlotModel _plotmodel { get; set; } = new PlotModel();
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
        public Balancetype CurrentBalanceType = new Balancetype();
        public string CurrentBalance { get; private set; }


        public HouseHoldAccountsPageViewModel(IHouseHoldAccounts ihouseholdaccounts)
        {
            /* デバッグ用 アイテム追加コマンド */
            this.ResistCommand = new ReactiveCommand();

            this._householdaccouts = ihouseholdaccounts;
            /* ReactiveProperty化(統計) */
            this.DisplayTotalIncome = _householdaccouts.ObserveProperty(h => h.SetTotalIncome).ToReactiveProperty().AddTo(disposable);
            this.DisplayTotalOutgoing = _householdaccouts.ObserveProperty(h => h.SetTotalOutgoing).ToReactiveProperty().AddTo(disposable);
            this.DisplayDifference = _householdaccouts.ObserveProperty(h => h.SetDifference).ToReactiveProperty().AddTo(disposable);
            this.DisplayTotalIncomeCollection = _householdaccouts.SetSIncomes.ToReadOnlyReactiveCollection(x => new VMHouseholdaccountScStatisticsItem(x)).AddTo(disposable);
            this.DisplayTotalOutgoingCollection = _householdaccouts.SetSOutgoings.ToReadOnlyReactiveCollection(x => new VMHouseholdaccountScStatisticsItem(x)).AddTo(disposable);

            /* ReactiveProperty化(グラフ) */
            this.Slices = _householdaccouts.SetPieSlice.ToReadOnlyReactiveCollection().AddTo(disposable);

            /* グラフの種類変更用のコマンドのインスタンス化 */
            this.SelectIncome = new ReactiveCommand();
            this.SelectOutgoing = new ReactiveCommand();
            this.SelectDifference = new ReactiveCommand();

            /* 日にちの初期化 */
            SelectedDate = new ReactiveProperty<DateTime>();
            SelectedDate.Value = DateTime.Today;

            SelectedRange = new ReactiveProperty<HouseholdaccoutRange>();

            DisplaySlices = new List<PieSlice>();

            /* ピッカー用のアイテムの作成 */
            RangeNames = new[]
            {
                new HouseholdaccoutRange
                {
                    RangeName = "統計:日単位",
                    R = Range.Day
                },
                new HouseholdaccoutRange
                {
                    RangeName = "統計:月単位" ,
                    R = Range.Month
                },
                new HouseholdaccoutRange
                {
                    RangeName = "統計:年単位",
                    R = Range.Year
                }
            };
            /* レンジを日単位で初期化 */
            SelectedRange.Value = RangeNames[0];

            /* 統計とグラフの初期化 */
            _householdaccouts.SetAllStatics(SelectedRange.Value.R, SelectedDate.Value);
            _householdaccouts.SetAllStaticsPie(SelectedRange.Value.R, CurrentBalanceType, SelectedDate.Value);

            /* グラフに表示するジャンルを支出で初期化 */
            CurrentBalanceType = Balancetype.outgoings;
            CurrentBalance = String.Format("支出 : {0}", DisplayTotalOutgoing.Value);
            _plotmodel.Title = CurrentBalance;

            // _plotmodel.InvalidatePlot(true);

            /*
            DisplaySlices.Clear();
            DisplaySlices.AddRange(Slices.Where(x => x.Price > 0).Select(x => new PieSlice(x.Label, x.Price) { Fill = OxyColor.Parse(x.ColorPath) }));
            */

            /* plotmodelにsliceを追加 */
            pieseries.Slices = DisplaySlices;
            this._plotmodel.Series.Add(pieseries);

            /* レンジが変更された時の処理 */
            SelectedRange.Subscribe(_ => 
            { if (_ != null)
                {
                    _householdaccouts.SetAllStatics(SelectedRange.Value.R, SelectedDate.Value);
                    _householdaccouts.SetAllStaticsPie(SelectedRange.Value.R, CurrentBalanceType, SelectedDate.Value);
                    CurrentBalance = BalanceTypeToTitleString(CurrentBalanceType);
                    PieUpdate();
                }
            })
            .AddTo(disposable);

            /* 日付が変更された時の処理 */
            SelectedDate.Subscribe(_ =>
            {
                if (_ != null) {
                    _householdaccouts.SetAllStatics(SelectedRange.Value.R, SelectedDate.Value);
                    _householdaccouts.SetAllStaticsPie(SelectedRange.Value.R, CurrentBalanceType, SelectedDate.Value);
                    CurrentBalance = BalanceTypeToTitleString(CurrentBalanceType);
                    PieUpdate();
                }
            })
            .AddTo(disposable);

            /* グラフの種類が収入に設定された時の処理 */
            SelectIncome.Subscribe(_ =>
            {
                CurrentBalanceType = Balancetype.incomes;
                CurrentBalance = BalanceTypeToTitleString(CurrentBalanceType);
                _householdaccouts.SetAllStaticsPie(SelectedRange.Value.R, CurrentBalanceType, SelectedDate.Value);
                PieUpdate();
            })
            .AddTo(disposable);

            /* グラフの種類が支出に設定された時の処理 */
            SelectOutgoing.Subscribe(_ =>
            {
                CurrentBalanceType = Balancetype.outgoings;
                CurrentBalance = BalanceTypeToTitleString(CurrentBalanceType);
                _householdaccouts.SetAllStaticsPie(SelectedRange.Value.R, CurrentBalanceType, SelectedDate.Value);
                PieUpdate();
            })
            .AddTo(disposable);

            /* グラフの種類が差分に設定された時の処理 */
            SelectDifference.Subscribe(_ =>
            {
                CurrentBalanceType = Balancetype.difference;
                CurrentBalance = BalanceTypeToTitleString(CurrentBalanceType);
                _householdaccouts.SetAllStaticsPie(SelectedRange.Value.R, CurrentBalanceType, SelectedDate.Value);
                PieUpdate();
            })
            .AddTo(disposable);

            /* デバッグ用 アイテム追加ボタンが押された時の処理 */
            ResistCommand.Subscribe(_ =>
            {
                DateTime temp = new DateTime(2017, 12, 16);
                _householdaccouts.AddHouseHoldAccountsItem("test1", 2, 100, temp, DCategorys.朝食, SCategorys.食費, StorageTypes.財布, true);
                _householdaccouts.AddHouseHoldAccountsItem("test2", 4, 300, DateTime.Today, DCategorys.消耗品, SCategorys.日用雑貨, StorageTypes.財布, true);
                _householdaccouts.AddHouseHoldAccountsItem("test3", 1, 500, DateTime.Today, DCategorys.ボーナス, SCategorys.給料, StorageTypes.財布, false);
                _householdaccouts.AddHouseHoldAccountsItem("test4", 1, 500, DateTime.Today, DCategorys.受取利息, SCategorys.投資収入, StorageTypes.財布, false);
                _householdaccouts.AddHouseHoldAccountsItem("test4", 1, 2000, temp, DCategorys.その他_収入, SCategorys.その他_収入, StorageTypes.財布, false);
                _householdaccouts.SetAllStatics(SelectedRange.Value.R, SelectedDate.Value);
                _householdaccouts.SetAllStaticsPie(SelectedRange.Value.R, CurrentBalanceType, SelectedDate.Value);
                CurrentBalance = BalanceTypeToTitleString(CurrentBalanceType);
                PieUpdate();
            }).AddTo(disposable);
        }

        /* 購読解除 */
        public void Dispose()
        {
            disposable.Dispose();
        }

        /* グラフの更新 */
        private void PieUpdate()
        {
            DisplaySlices.Clear();
            DisplaySlices.AddRange(Slices.Where(x => x.Price > 0).Select(x => new PieSlice(null, x.Price) { Fill = OxyColor.Parse(x.ColorPath) }));
            // pieseries.Slices = DisplaySlices;
            // _plotmodel.Series.Add(pieseries);
            _plotmodel.Title = CurrentBalance;
            DisplayPlotModel = _plotmodel;
            DisplayPlotModel.InvalidatePlot(true);
            DisplayPlotModel.InvalidatePlot(true);
            DisplayPlotModel.InvalidatePlot(true);
            DisplayPlotModel.InvalidatePlot(true);
        }

        public string BalanceTypeToTitleString(Balancetype bc)
        {
            switch (bc)
            {
                case Balancetype.incomes:
                    return String.Format("収入 : {0}", DisplayTotalIncome.Value);
                case Balancetype.outgoings:
                    return  String.Format("支出 : {0}", DisplayTotalOutgoing.Value);
                case Balancetype.difference:
                    return String.Format("差分 : {0}", DisplayDifference.Value);
                default:
                    return null;
            }
        }
    }
}
