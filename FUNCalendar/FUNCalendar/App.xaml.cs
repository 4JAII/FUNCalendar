﻿using Prism.Unity;
using FUNCalendar.Views;
using FUNCalendar.Models;
using FUNCalendar.Services;
using System.Collections.Generic;
using Xamarin.Forms;
using Microsoft.Practices.Unity;

namespace FUNCalendar
{
    public partial class App : PrismApplication
    {
        public App(IPlatformInitializer initializer = null) : base(initializer) { }

        protected override void OnInitialized()
        {
            InitializeComponent();

            NavigationService.NavigateAsync("/RootPage/NavigationPage/CalendarPage");
        }

        protected override void RegisterTypes()
        {
            /* 画面をDIコンテナに登録 */
            Container.RegisterTypeForNavigation<RootPage>();
            Container.RegisterTypeForNavigation<NavigationPage>();
            Container.RegisterTypeForNavigation<MenuPage>();
            Container.RegisterTypeForNavigation<CalendarPage>();
            Container.RegisterTypeForNavigation<WishListPage>();
            Container.RegisterTypeForNavigation<ToDoListPage>();
            Container.RegisterTypeForNavigation<WishListRegisterPage>();
            Container.RegisterTypeForNavigation<HouseHoldAccountsPage>();
            /* 共有のインスタンスをDIコンテナに登録 */
            Container.RegisterType<IWishList, WishList>(new ContainerControlledLifetimeManager());
            Container.RegisterType<IStorageService, StorageService>(new ContainerControlledLifetimeManager());

            /* Householdaccounts */
            Container.RegisterType<IHouseHoldAccounts, HouseHoldAccounts>(new ContainerControlledLifetimeManager());
            Container.RegisterTypeForNavigation<HouseHoldAccountsStatisticsPage>();
            Container.RegisterTypeForNavigation<HouseHoldAccountsSCStatisticsPage>();
            Container.RegisterTypeForNavigation<HouseholdaccountsDCHistoryPage>();
            Container.RegisterTypeForNavigation<HouseholdaccountBalancePage>();
            Container.RegisterTypeForNavigation<HouseholdaccountsHistoryPage>();
            Container.RegisterTypeForNavigation<HouseholdaccountsRegisterPage>();
        }
    }
}
