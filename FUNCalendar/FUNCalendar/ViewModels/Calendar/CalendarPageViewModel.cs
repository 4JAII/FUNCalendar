using Prism.Mvvm;
using Prism.Navigation;
using Reactive.Bindings;
using FUNCalendar.Models;
using FUNCalendar.Services;
using System;
using System.Reactive.Disposables;
using System.Collections.Generic;
using Reactive.Bindings.Extensions;
using Prism.Services;
using System.Collections.ObjectModel;
using Xamarin.Forms;
using System.Reactive.Threading;
using System.Reactive;
using System.Reactive.Linq;

namespace FUNCalendar.ViewModels
{
    public class CalendarPageViewModel : BindableBase, INavigationAware, IDisposable
    {
        /* 全てのリストを初期化 */
        private static ReactiveProperty<bool> canInitialize = new ReactiveProperty<bool>();
        private IStorageService _storageService;
        private ILoadingMessage _loadingMessage;

        private IWishList _wishList;
        private IToDoList _todoList;
        private IHouseholdAccounts _householdAccounts;
        private ICalendar _calendar;
        private IPageDialogService _pageDialogService;
        private INavigationService _navigationService;

        /* 表示用データ */
        private string currentMonth;
        public string CurrentMonth
        {
            get { return this.currentMonth; }
            set { this.SetProperty(ref this.currentMonth, value); }
        }
        private string currentYear;
        public string CurrentYear
        {
            get { return this.currentYear; }
            set { this.SetProperty(ref this.currentYear, value); }
        }
        private string currentDate;
        public string CurrentDate
        {
            get { return this.currentDate; }
            set { this.SetProperty(ref this.currentDate, value); }
        }
        private string calendarYear;
        public string CalendarYear
        {
            get { return this.calendarYear; }
            set { this.SetProperty(ref this.calendarYear, value); }
        }
        private string calendarMonth;
        public string CalendarMonth
        {
            get { return this.calendarMonth; }
            set { this.SetProperty(ref this.calendarMonth, value); }
        }
        /* 一か月の収入の合計 */
        public ReactiveProperty<string> MonthIncome { get; private set; } = new ReactiveProperty<string>();
        /* 一か月の支出の合計 */
        public ReactiveProperty<string> MonthOutgoing { get; private set; } = new ReactiveProperty<string>();

        public int SelectedYear;
        public int SelectedMonth;
        public DateTime SelectedDate { get; private set; }

        public ReactiveProperty<bool> IsEndRefreshing { get; private set; } = new ReactiveProperty<bool>();

        public ReactiveCommand TapCommand { get; private set; }
        public ReactiveCommand BackPrevMonth { get; private set; }
        public ReactiveCommand GoNextMonth { get; private set; }

        /* 表示用リスト */
        public ReadOnlyReactiveCollection<VMDate> DisplayCalendar { get; private set; }
        /* 画面遷移用 */
        public AsyncReactiveCommand NavigationRegisterPageCommand { get; private set; }
        /* 購読解除用 */
        private CompositeDisposable Disposable { get; } = new CompositeDisposable();

        public CalendarPageViewModel(ILoadingMessage loadingMessage, IWishList wishList, IToDoList todoList, IHouseholdAccounts householdAccounts, IStorageService storageService, ICalendar calendar, INavigationService navigationService, IPageDialogService pageDialogService)
        {
            this._storageService = storageService;
            this._loadingMessage = loadingMessage;
            this._calendar = calendar;
            this._wishList = wishList;
            this._todoList = todoList;
            this._householdAccounts = householdAccounts;
            
            canInitialize.Subscribe(async _ =>
            {
                await _storageService.InitializeAsync(this._wishList, this._todoList, this._householdAccounts);
                await _storageService.ReadFile();
                _calendar.SetLists(_wishList, _todoList, _householdAccounts);
                _householdAccounts.SetMonthBalance(SelectedDate);
            });

            this._pageDialogService = pageDialogService;
            this._navigationService = navigationService;
            NavigationRegisterPageCommand = new AsyncReactiveCommand();

            TapCommand = new ReactiveCommand();
            BackPrevMonth = new ReactiveCommand();
            GoNextMonth = new ReactiveCommand();

            CalendarYear = string.Format("{0}年", _calendar.CurrentYear.ToString());
            CalendarMonth = string.Format("{0}月", _calendar.CurrentMonth.ToString());
            CurrentYear = string.Format("{0}年", DateTime.Now.ToString("yyyy"));
            CurrentMonth = string.Format("{0}月", DateTime.Now.ToString("%M"));
            CurrentDate = string.Format("{0}日", DateTime.Now.ToString("%d"));

            MonthIncome = _householdAccounts.ObserveProperty(h => h.IncomeForCalendar).Select(i => string.Format("収入：{0}",i)).ToReactiveProperty().AddTo(Disposable);
            MonthOutgoing = _householdAccounts.ObserveProperty(h => h.OutgoingForCalendar).Select(i => string.Format("支出：{0}",i)).ToReactiveProperty().AddTo(Disposable);

            SelectedYear = _calendar.CurrentYear;
            SelectedMonth = _calendar.CurrentMonth;
            SelectedDate = DateTime.Parse(String.Format("{0}/{1}", SelectedYear, SelectedMonth));


            DisplayCalendar = _calendar.ListedAMonthDateData.ToReadOnlyReactiveCollection(x => new VMDate(x)).AddTo(Disposable);
            IsEndRefreshing.Value = true;
            DisplayCalendar.ObserveAddChanged().Buffer(42).Subscribe(_ =>
            {
                IsEndRefreshing.Value = true;
                _loadingMessage.Hide();
            });

            TapCommand.Subscribe(async (obj) =>
            {
                _calendar.SetDisplayDate(VMDate.ToDate(obj as VMDate));
                await _navigationService.NavigateAsync($"/RootPage/NavigationPage/CalendarDetailPage");
            });

            BackPrevMonth.Subscribe(() =>
            {
                if (!IsEndRefreshing.Value) return;
                IsEndRefreshing.Value = false;
                _loadingMessage.Show("読み込み中");
                _calendar.BackPrevMonth();
                SelectedYear = _calendar.CurrentYear;
                SelectedMonth = _calendar.CurrentMonth;
                SelectedDate = DateTime.Parse(String.Format("{0}/{1}", SelectedYear, SelectedMonth));
                _householdAccounts.SetMonthBalance(SelectedDate);
                CalendarYear = string.Format("{0}年", _calendar.CurrentYear.ToString());
                CalendarMonth = string.Format("{0}月", _calendar.CurrentMonth.ToString());
            });

            GoNextMonth.Subscribe(() =>
            {
                if (!IsEndRefreshing.Value) return;
                IsEndRefreshing.Value = false;
                _loadingMessage.Show("読み込み中");
                _calendar.GoNextMonth();
                var selectedyear = _calendar.CurrentYear;
                var selectedmonth = _calendar.CurrentMonth;
                SelectedDate = DateTime.Parse(String.Format("{0}/{1}", selectedyear, selectedmonth));
                _householdAccounts.SetMonthBalance(SelectedDate);
                CalendarYear = string.Format("{0}年", _calendar.CurrentYear.ToString());
                CalendarMonth = string.Format("{0}月", _calendar.CurrentMonth.ToString());
            });

            NavigationRegisterPageCommand.Subscribe(async () =>
            {
                var result = await _pageDialogService.DisplayActionSheetAsync("登録するアイテムの種類を選択", "キャンセル", "", "ToDo", "WishList", "家計簿");
                var navigationParameters = new NavigationParameters();
                switch (result)
                {
                    case "ToDo":
                        navigationParameters.Add("BackPage", "/RootPage/NavigationPage/CalendarPage");
                        await this._navigationService.NavigateAsync($"/NavigationPage/ToDoListRegisterPage", navigationParameters);
                        break;
                    case "WishList":
                        navigationParameters.Add("BackPage", "/RootPage/NavigationPage/CalendarPage");
                        await this._navigationService.NavigateAsync($"/NavigationPage/WishListRegisterPage", navigationParameters);
                        break;
                    case "家計簿":
                        var navigationitem = new HouseholdAccountsNavigationItem(DateTime.Today);
                        navigationParameters.Add(HouseholdAccountsRegisterPageViewModel.CalendarKey, navigationitem);
                        navigationParameters.Add("BackPage", PageName.CalendarPage);
                        await this._navigationService.NavigateAsync($"/NavigationPage/HouseholdAccountsRegisterPage", navigationParameters);
                        break;
                }
            });

            
        }

        public void OnNavigatedFrom(NavigationParameters parameters)
        {

        }

        public void OnNavigatedTo(NavigationParameters parameters)
        {
            canInitialize.Value = true;
        }

        public void OnNavigatingTo(NavigationParameters parameters)
        {

        }

        public void Dispose()
        {
            Disposable.Dispose();
        }
    }
}
