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
    public class HouseholdAccountsHistoryPageViewModel : BindableBase, INavigationAware, IDisposable
    {
        private IHouseholdAccounts _householdaccounts;
        private IStorageService _storageService;
        private INavigationService _navigationService;
        private IPageDialogService _pageDialogService;

        public static readonly string InputKey = "InputKey";

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

        public AsyncReactiveCommand ResistCommand { get; private set; }

        private CompositeDisposable disposable { get; } = new CompositeDisposable();

        /* 統計画面移行用 */
        public AsyncReactiveCommand StatisticsCommand { get; private set; }

        /* 残高画面移行用 */
        public AsyncReactiveCommand BalanceCommand { get; private set; }

        /* 編集コマンド */
        public ReactiveCommand EditCommand { get; private set; }

        /* 削除コマンド */
        public ReactiveCommand RemoveCommand { get; private set; }


        public HouseholdAccountsHistoryPageViewModel(IHouseholdAccounts householdaccounts, IStorageService storageService, INavigationService navigationService, IPageDialogService pageDialogService)
        {
            this._householdaccounts = householdaccounts;
            this._storageService = storageService;
            this._navigationService = navigationService;
            this._pageDialogService = pageDialogService;

            DisplayHistoryCollection = _householdaccounts.DisplayHouseholdaccountList.ToReadOnlyReactiveCollection(x => new VMHouseholdAccountsItem(x)).AddTo(disposable);

            /* インスタンス化 */
            SelectedRange = new ReactiveProperty<HouseholdAccountsRangeItem>();
            SelectedDate = new ReactiveProperty<DateTime>();
            BalanceCommand = new AsyncReactiveCommand();
            StatisticsCommand = new AsyncReactiveCommand();
            ResistCommand = new AsyncReactiveCommand();
            EditCommand = new ReactiveCommand();
            RemoveCommand = new ReactiveCommand();


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

            /* アイテム追加ボタンが押された時の処理 */
            ResistCommand.Subscribe(async _ =>
            {
                var navigationitem = new HouseholdAccountsNavigationItem(SelectedDate.Value, SelectedRange.Value.RangeData);
                var navigationparameter = new NavigationParameters()
                {
                    {HouseholdAccountsRegisterPageViewModel.InputKey, navigationitem }
                };
                navigationparameter.Add("BackPage", PageName.HouseholdAccountsHistoryPage);
                await _navigationService.NavigateAsync("/RootPage/NavigationPage/HouseholdAccountsRegisterPage", navigationparameter);
            }).AddTo(disposable);

            /* アイテムを編集するときの処理 */
            EditCommand.Subscribe(async (obj) =>
            {
                _householdaccounts.SetHouseholdAccountsItem(VMHouseholdAccountsItem.ToHouseholdaccountsItem(obj as VMHouseholdAccountsItem));
                var navigationitem = new HouseholdAccountsNavigationItem(SelectedDate.Value, SelectedRange.Value.RangeData);
                var navigationparameter = new NavigationParameters()
                {
                    {HouseholdAccountsRegisterPageViewModel.EditKey, navigationitem }
                };
                navigationparameter.Add("BackPage", PageName.HouseholdAccountsHistoryPage);
                await _navigationService.NavigateAsync("/RootPage/NavigationPage/HouseholdAccountsRegisterPage", navigationparameter);
            });

            /* アイテムを削除するときの処理 */
            RemoveCommand.Subscribe(async (obj) =>
            {
                var result = await _pageDialogService.DisplayAlertAsync("確認", "削除しますか？", "はい", "いいえ");
                if (result)
                {
                    await _storageService.DeleteItem(VMHouseholdAccountsItem.ToHouseholdaccountsItem(obj as VMHouseholdAccountsItem));
                    _householdaccounts.SetAllHistory(SelectedRange.Value.RangeData, SelectedDate.Value);
                }
            });

            /* 統計ボタンが押されたときの処理 */
            StatisticsCommand.Subscribe(async _ =>
            {
                var navigationitem = new HouseholdAccountsNavigationItem(SelectedDate.Value, SelectedRange.Value.RangeData);
                var navigationparameter = new NavigationParameters()
                {
                    {HouseholdAccountsBalancePageViewModel.InputKey, navigationitem }
                };
                await _navigationService.NavigateAsync("/RootPage/NavigationPage/HouseholdAccountsStatisticsPage", navigationparameter);
            }).AddTo(disposable);


            /* 残高ボタンが押されたときの処理 */
            BalanceCommand.Subscribe(async _ =>
            {
                var navigationitem = new HouseholdAccountsNavigationItem(SelectedDate.Value, SelectedRange.Value.RangeData);
                var navigationparameter = new NavigationParameters()
                {
                    {HouseholdAccountsBalancePageViewModel.InputKey, navigationitem }
                };
                await _navigationService.NavigateAsync("/RootPage/NavigationPage/HouseholdAccountsBalancePage", navigationparameter);
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
                _householdaccounts.SetAllHistory(SelectedRange.Value.RangeData, SelectedDate.Value);

                /* 日付が変更された時の処理 */
                SelectedDate.Subscribe(_ =>
                {
                    if (_ != null)
                    {
                        _householdaccounts.SetAllHistory(SelectedRange.Value.RangeData, SelectedDate.Value);
                    }
                })
                .AddTo(disposable);

                /* レンジが変更された時の処理 */
                SelectedRange.Subscribe(_ =>
                {
                    if (_ != null)
                    {
                        _householdaccounts.SetAllHistory(SelectedRange.Value.RangeData, SelectedDate.Value);
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
