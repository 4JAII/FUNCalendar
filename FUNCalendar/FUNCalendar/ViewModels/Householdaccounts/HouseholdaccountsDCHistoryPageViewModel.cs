using FUNCalendar.Models;
using Prism.Mvvm;
using Prism.Navigation;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Prism.Services;
using FUNCalendar.Services;

namespace FUNCalendar.ViewModels
{
    public class HouseholdAccountsDCHistoryPageViewModel : BindableBase, INavigationAware, IDisposable
    {
        public AsyncReactiveCommand BackPageCommand { get; private set; }

        private IHouseholdAccounts _householdaccounts;
        private INavigationService _navigationService;
        private IStorageService _storageService;
        private IPageDialogService _pageDialogService;

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

        /* アイテム追加コマンド */
        public AsyncReactiveCommand ResistCommand { get; private set; }

        /* 編集コマンド */
        public ReactiveCommand EditCommand { get; private set; }

        /* 削除コマンド */
        public ReactiveCommand RemoveCommand { get; private set; }

        /* 監視解除 */
        private CompositeDisposable disposable { get; } = new CompositeDisposable();

        public HouseholdAccountsDCHistoryPageViewModel(IHouseholdAccounts householdaccounts, IStorageService storageService, INavigationService navigationService, IPageDialogService pageDialogService)
        {
            this._householdaccounts = householdaccounts;
            this._storageService = storageService;
            this._navigationService = navigationService;
            this._pageDialogService = pageDialogService;

            this.ResistCommand = new AsyncReactiveCommand();

            DisplayHistoryCollection = _householdaccounts.DisplayHouseholdaccountList.ToReadOnlyReactiveCollection(x => new VMHouseholdAccountsItem(x)).AddTo(disposable);

            /* インスタンス化 */
            this.BackPageCommand = new AsyncReactiveCommand();
            EditCommand = new ReactiveCommand();
            RemoveCommand = new ReactiveCommand();
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
            BackPageCommand.Subscribe(async _ =>
            {
                var navigationitem = new HouseholdAccountsNavigationItem(CurrentBalanceType, CurrentSCategory, SelectedDate.Value, SelectedRange.Value.RangeData);
                var navigationparameter = new NavigationParameters()
                {
                    {HouseholdAccountsStatisticsPageViewModel.InputKey, navigationitem }
                };
                await _navigationService.NavigateAsync("/RootPage/NavigationPage/HouseholdAccountsSCStatisticsPage", navigationparameter);
            }).AddTo(disposable);


            /* アイテム追加ボタンが押された時の処理 */
            ResistCommand.Subscribe(async _ =>
            {
                var navigationitem = new HouseholdAccountsNavigationItem(CurrentBalanceType, CurrentSCategory, CurrentDCategory, SelectedDate.Value, SelectedRange.Value.RangeData);
                var navigationparameter = new NavigationParameters()
                {
                    {HouseholdAccountsRegisterPageViewModel.InputKey, navigationitem }
                };
                navigationparameter.Add("BackPage", PageName.HouseholdAccountsDCHistoryPage);
                await _navigationService.NavigateAsync("/RootPage/NavigationPage/HouseholdAccountsRegisterPage", navigationparameter);
            }).AddTo(disposable);

            /* アイテムを編集するときの処理 */
            EditCommand.Subscribe(async (obj) =>
            {
                _householdaccounts.SetHouseholdAccountsItem(VMHouseholdAccountsItem.ToHouseholdaccountsItem(obj as VMHouseholdAccountsItem));
                var navigationitem = new HouseholdAccountsNavigationItem(CurrentBalanceType, CurrentSCategory, SelectedDate.Value, SelectedRange.Value.RangeData);
                var navigationparameter = new NavigationParameters()
                {
                    {HouseholdAccountsRegisterPageViewModel.EditKey, navigationitem }
                };
                navigationparameter.Add("BackPage", PageName.HouseholdAccountsDCHistoryPage);
                await _navigationService.NavigateAsync("/RootPage/NavigationPage/HouseholdAccountsRegisterPage", navigationparameter);
            });

            /* アイテムを削除するときの処理 */
            RemoveCommand.Subscribe(async (obj) =>
            {
                var result = await _pageDialogService.DisplayAlertAsync("確認", "削除しますか？", "はい", "いいえ");
                if (result)
                {
                    await _storageService.DeleteItem(VMHouseholdAccountsItem.ToHouseholdaccountsItem(obj as VMHouseholdAccountsItem));
                    _householdaccounts.SetDCategoryHistory(SelectedRange.Value.RangeData, SelectedDate.Value, CurrentDCategory);
                }
            });
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
