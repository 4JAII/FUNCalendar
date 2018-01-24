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
    public class HouseholdAccountsStatisticsPageViewModel : BindableBase, INavigationAware, IDisposable
    {
        private IHouseholdAccounts _householdaccounts;                           /* 家計簿のinterface */
        private INavigationService _inavigationservice;

        /* 正しい遷移か確認するためのkey */
        public static readonly string InputKey = "InputKey";

        /* 遷移されたときの格納用変数 */
        public HouseholdAccountsNavigationItem NavigatedItem { get; set; }

        public ReactiveProperty<DateTime> SelectedDate { get; private set; }    /* 日付を格納 */
        public ReactiveProperty<HouseholdAccountsRangeItem> SelectedRange { get; private set; }   /* 範囲を格納 */

        public ReactiveProperty<string> DisplayTotalIncome { get; private set; }            /* 収入の合計値を格納 */
        public ReactiveProperty<string> DisplayTotalOutgoing { get; private set; }          /* 支出の合計値を格納 */
        public ReactiveProperty<string> DisplayDifference { get; private set; }             /* 差分地を格納 */
        public ReadOnlyReactiveCollection<VMHouseholdAccountsScStatisticsItem> DisplayTotalIncomeCollection { get; private set; }        /* 収入の概要カテゴリーの合計値を格納 */
        public ReadOnlyReactiveCollection<VMHouseholdAccountsScStatisticsItem> DisplayTotalOutgoingCollection { get; private set; }      /* 支出の概要カテゴリーの合計値を格納 */


        /* デバッグ用 */
        public AsyncReactiveCommand ResistCommand { get; private set; }

        /* Picker用のアイテム */
        public HouseholdAccountsRangeItem[] RangeNames { get; private set; }

        private CompositeDisposable disposable { get; } = new CompositeDisposable();

        /* グラフ */
        public ReactiveCommand SelectOutgoing { get; set; }
        public ReactiveCommand SelectIncome { get; private set; }
        public ReactiveCommand SelectDifference { get; private set; }
        private PlotModel _plotmodel { get; set; } = new PlotModel();
        public PlotModel DisplayPlotModel { get; private set; }
        public ReadOnlyReactiveCollection<HouseholdAccountsPieSliceItem> Slices { get; private set; }
        public List<PieSlice> DisplaySlices { get; private set; }
        private PieSeries pieseries = new PieSeries()
        {
            StrokeThickness = 2.0,
            InsideLabelPosition = 0.5,
            AngleSpan = 360,
            StartAngle = 270,
            InnerDiameter = 0.8
        };
        public BalanceTypes CurrentBalanceType = new BalanceTypes();
        public string CurrentBalance { get; private set; }
        public ReadOnlyReactiveCollection<VMHouseholdAccountsLegendItem> DisplayLegend { get; set; }

        /* 詳細画面移行用 */
        public Command<VMHouseholdAccountsScStatisticsItem> ItemSelectedCommand { get; }

        /* 残高画面移行用 */
        public AsyncReactiveCommand BalanceCommand { get; private set; }

        /* 履歴画面移行用 */
        public AsyncReactiveCommand HistoryCommand { get; private set; }

        /* 編集用 */
        public AsyncReactiveCommand EditCommand { get; private set; }



        public HouseholdAccountsStatisticsPageViewModel(IHouseholdAccounts ihouseholdaccounts, INavigationService navigationService)
        {

            this._householdaccounts = ihouseholdaccounts;
            this._inavigationservice = navigationService;

            /* ReactiveProperty化(統計) */
            this.DisplayTotalIncome = _householdaccounts.ObserveProperty(h => h.TotalIncome).ToReactiveProperty().AddTo(disposable);
            this.DisplayTotalOutgoing = _householdaccounts.ObserveProperty(h => h.TotalOutgoing).ToReactiveProperty().AddTo(disposable);
            this.DisplayDifference = _householdaccounts.ObserveProperty(h => h.Difference).ToReactiveProperty().AddTo(disposable);
            this.DisplayTotalIncomeCollection = _householdaccounts.SIncomes.ToReadOnlyReactiveCollection(x => new VMHouseholdAccountsScStatisticsItem(x)).AddTo(disposable);
            this.DisplayTotalOutgoingCollection = _householdaccounts.SOutgoings.ToReadOnlyReactiveCollection(x => new VMHouseholdAccountsScStatisticsItem(x)).AddTo(disposable);

            /* ReactiveProperty化(グラフ) */
            this.Slices = _householdaccounts.PieSlice.ToReadOnlyReactiveCollection().AddTo(disposable);

            /* RectiveProperty化(グラフ凡例) */
            this.DisplayLegend = _householdaccounts.Legend.ToReadOnlyReactiveCollection(x => new VMHouseholdAccountsLegendItem(x)).AddTo(disposable);

            /* グラフの種類変更用のコマンドのインスタンス化 */
            this.SelectIncome = new ReactiveCommand();
            this.SelectOutgoing = new ReactiveCommand();
            this.SelectDifference = new ReactiveCommand();

            /* インスタンス化 */
            SelectedDate = new ReactiveProperty<DateTime>();
            SelectedDate.Value = DateTime.Today;
            SelectedRange = new ReactiveProperty<HouseholdAccountsRangeItem>();
            BalanceCommand = new AsyncReactiveCommand();
            HistoryCommand = new AsyncReactiveCommand();
            ResistCommand = new AsyncReactiveCommand();
            EditCommand = new AsyncReactiveCommand();
            DisplaySlices = new List<PieSlice>();

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
            /* レンジを日単位で初期化 */
            SelectedRange.Value = RangeNames[0];

            /* 統計とグラフの初期化 */
            _householdaccounts.SetAllStatics(SelectedRange.Value.RangeData, SelectedDate.Value);
            _householdaccounts.SetAllStaticsPie(SelectedRange.Value.RangeData, CurrentBalanceType, SelectedDate.Value);

            /* グラフに表示するジャンルを支出で初期化 */
            CurrentBalanceType = BalanceTypes.outgoings;
            CurrentBalance = String.Format("支出 : {0}", DisplayTotalOutgoing.Value);
            _plotmodel.Title = CurrentBalance;

            /* plotmodelにsliceを追加 */
            pieseries.Slices = DisplaySlices;
            this._plotmodel.Series.Add(pieseries);
            UpdatePie();


            /* レンジが変更された時の処理 */
            SelectedRange.Subscribe(_ =>
            {
                if (_ != null)
                {
                    _householdaccounts.SetAllStatics(SelectedRange.Value.RangeData, SelectedDate.Value);
                    _householdaccounts.SetAllStaticsPie(SelectedRange.Value.RangeData, CurrentBalanceType, SelectedDate.Value);
                }
            })
            .AddTo(disposable);

            /* 日付が変更された時の処理 */
            SelectedDate.Subscribe(_ =>
            {
                if (_ != null)
                {
                    _householdaccounts.SetAllStatics(SelectedRange.Value.RangeData, SelectedDate.Value);
                    _householdaccounts.SetAllStaticsPie(SelectedRange.Value.RangeData, CurrentBalanceType, SelectedDate.Value);
                }
            })
            .AddTo(disposable);

            /* グラフの種類が収入に設定された時の処理 */
            SelectIncome.Subscribe(_ =>
            {
                CurrentBalanceType = BalanceTypes.incomes;
                _householdaccounts.SetAllStaticsPie(SelectedRange.Value.RangeData, CurrentBalanceType, SelectedDate.Value);
            });

            /* グラフの種類が支出に設定された時の処理 */
            SelectOutgoing.Subscribe(_ =>
            {
                CurrentBalanceType = BalanceTypes.outgoings;
                _householdaccounts.SetAllStaticsPie(SelectedRange.Value.RangeData, CurrentBalanceType, SelectedDate.Value);
            })
            .AddTo(disposable);

            /* グラフの種類が差分に設定された時の処理 */
            SelectDifference.Subscribe(_ =>
            {
                CurrentBalanceType = BalanceTypes.difference;
                _householdaccounts.SetAllStaticsPie(SelectedRange.Value.RangeData, CurrentBalanceType, SelectedDate.Value);
            })
            .AddTo(disposable);

            /* アイテム追加ボタンが押された時の処理 */
            ResistCommand.Subscribe(async _ =>
            {
                var navigationitem = new HouseholdAccountsNavigationItem(SelectedDate.Value, SelectedRange.Value.RangeData);
                var navigationparameter = new NavigationParameters()
                {
                    {HouseholdAccountsRegisterPageViewModel.InputKey, navigationitem }
                };
                navigationparameter.Add("BackPage", "/RootPage/NavigationPage/HouseholdAccountsStatisticsPage");
                await _inavigationservice.NavigateAsync("/RootPage/NavigationPage/HouseholdAccountsRegisterPage",navigationparameter);
            }).AddTo(disposable);

            /* グラフのデータが変更された時の処理 */
            Slices.CollectionChangedAsObservable().Subscribe(_ => { UpdatePie(); });

            /* リストのアイテムがクリックされた時の処理 */
            ItemSelectedCommand = new Command<VMHouseholdAccountsScStatisticsItem>(x =>
            {
                var currentbalancetype = (BalanceTypes)Enum.Parse(typeof(BalanceTypes), x.Balancetype);
                var currentscategory = (SCategorys)Enum.Parse(typeof(SCategorys), x.Scategory);
                var navigationitem = new HouseholdAccountsNavigationItem(currentbalancetype, currentscategory, SelectedDate.Value, SelectedRange.Value.RangeData);
                var navigationparameter = new NavigationParameters()
                {
                    {HouseholdAccountsSCStatisticsPageViewModel.InputKey, navigationitem }
                };
                _inavigationservice.NavigateAsync("/RootPage/NavigationPage/HouseholdAccountsSCStatisticsPage", navigationparameter);
            });

            /* アイテムを編集するときの処理 */
            EditCommand.Subscribe(async (obj) =>
            {
                _householdaccounts.SetHouseholdAccountsItem(VMHouseholdAccountsItem.ToHouseholdaccountsItem(obj as VMHouseholdAccountsItem));
                var navigationitem = new HouseholdAccountsNavigationItem(SelectedDate.Value, SelectedRange.Value.RangeData);
                var navigationparameter = new NavigationParameters()
                {
                    {HouseholdAccountsRegisterPageViewModel.InputKey, navigationitem }
                };
                navigationparameter.Add("BackPage", "/RootPage/NavigationPage/HouseholdAccountsStatisticsPage");
                await _inavigationservice.NavigateAsync("/RootPage/NavigationPage/HouseholdAccountsRegisterPage", navigationparameter);
            });

            /* 残高ボタンが押されたときの処理 */
            BalanceCommand.Subscribe(async _ =>
            {
                var navigationitem = new HouseholdAccountsNavigationItem(SelectedDate.Value, SelectedRange.Value.RangeData);
                var navigationparameter = new NavigationParameters()
                {
                    {HouseholdAccountsBalancePageViewModel.InputKey, navigationitem }
                };
                await _inavigationservice.NavigateAsync("/RootPage/NavigationPage/HouseholdAccountsBalancePage", navigationparameter);
            }).AddTo(disposable);

            /* 履歴ボタンが押されたときの処理 */
            HistoryCommand.Subscribe(async _ =>
            {
                var navigationitem = new HouseholdAccountsNavigationItem(SelectedDate.Value, SelectedRange.Value.RangeData);
                var navigationparameter = new NavigationParameters()
                {
                    {HouseholdAccountsBalancePageViewModel.InputKey, navigationitem }
                };
                await _inavigationservice.NavigateAsync("/RootPage/NavigationPage/HouseholdAccountsHistoryPage", navigationparameter);
            }).AddTo(disposable);

        }

        /* 購読解除 */
        public void Dispose()
        {
            disposable.Dispose();
        }

        /* グラフの更新 */
        private void UpdatePie()
        {
            DisplaySlices.Clear();
            DisplaySlices.AddRange(Slices.Where(x => x.Price > 0).Select(x => new PieSlice(null, x.Price) { Fill = OxyColor.Parse(x.ColorPath) }));
            CurrentBalance = BalanceTypeToTitleString(CurrentBalanceType);
            _plotmodel.Title = CurrentBalance;
            DisplayPlotModel = _plotmodel;
            DisplayPlotModel.InvalidatePlot(true);
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

                CurrentBalanceType = BalanceTypes.outgoings;
                _householdaccounts.SetAllStatics(SelectedRange.Value.RangeData, SelectedDate.Value);
                _householdaccounts.SetAllStaticsPie(SelectedRange.Value.RangeData, CurrentBalanceType, SelectedDate.Value);
            }
        }

        public void OnNavigatingTo(NavigationParameters parameters)
        {
        }

        /* グラフタイトルの設定 */
        public string BalanceTypeToTitleString(BalanceTypes bc)
        {
            switch (bc)
            {
                case BalanceTypes.incomes:
                    return String.Format("収入 : {0}", DisplayTotalIncome.Value);
                case BalanceTypes.outgoings:
                    return String.Format("支出 : {0}", DisplayTotalOutgoing.Value);
                case BalanceTypes.difference:
                    return String.Format("差分 : {0}", DisplayDifference.Value);
                default:
                    return null;
            }
        }

    }
}
