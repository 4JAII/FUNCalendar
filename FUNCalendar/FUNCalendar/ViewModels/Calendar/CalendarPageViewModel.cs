﻿using Prism.Mvvm;
using Prism.Navigation;
using Reactive.Bindings;
using FUNCalendar.Models;
using System;
using System.Reactive.Disposables;
using System.Collections.Generic;
using Reactive.Bindings.Extensions;
using Prism.Services;
using System.Collections.ObjectModel;

namespace FUNCalendar.ViewModels
{
    public class CalendarPageViewModel : BindableBase, INavigationAware, IDisposable
    {
        /* 全てのリストを初期化 */
        private static ReactiveProperty<bool> canInitializeList= new ReactiveProperty<bool>();
        private LocalStorage localStorage = new LocalStorage();

        private ICalendar _calendar;
        private IWishList _wishList;
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

        public ReactiveCommand TapCommand { get; private set; }
        public ReactiveCommand BackPrevMonth { get; private set; }
        public ReactiveCommand GoNextMonth { get; private set; }

        /* 表示用リスト */
        public ReadOnlyReactiveCollection<VMDate> DisplayCalendar { get; private set; }
        /* 画面遷移用 */
        public AsyncReactiveCommand NavigationRegisterPageCommand { get; private set; }
        /* 購読解除用 */
        private CompositeDisposable Disposable { get; } = new CompositeDisposable();

        public CalendarPageViewModel(ICalendar calendar, IWishList wishList, INavigationService navigationService, IPageDialogService pageDialogService)
        {
            this._wishList = wishList;

            canInitializeList.Subscribe(async _ =>
            {
                _wishList.InitializeList(await localStorage.ReadFile());
            });

            /* 以下、変更箇所 */
            this._calendar = calendar;
            _calendar.SetHasList(_wishList);
            
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

            DisplayCalendar = _calendar.ListedAMonthDateData.ToReadOnlyReactiveCollection(x => new VMDate(x)).AddTo(Disposable);

            TapCommand.Subscribe(async (obj) =>
            {
                _calendar.SetDisplayDate(VMDate.ToDate(obj as VMDate));
                await _navigationService.NavigateAsync($"/RootPage/NavigationPage/CalendarDetailPage");
            });

            BackPrevMonth.Subscribe(() =>
            {
                _calendar.BackPrevMonth();
                _calendar.SetHasList(_wishList);
                CalendarYear = string.Format("{0}年", _calendar.CurrentYear.ToString());
                CalendarMonth = string.Format("{0}月", _calendar.CurrentMonth.ToString());
            });

            GoNextMonth.Subscribe(() =>
            {
                _calendar.GoNextMonth();
                _calendar.SetHasList(_wishList);
                CalendarYear = string.Format("{0}年", _calendar.CurrentYear.ToString());
                CalendarMonth = string.Format("{0}月", _calendar.CurrentMonth.ToString());
            });
        }

        public void OnNavigatedFrom(NavigationParameters parameters)
        {

        }

        public void OnNavigatedTo(NavigationParameters parameters)
        {
            canInitializeList.Value = true;
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