using Prism.Unity;
using FUNCalendar.Views;
<<<<<<< HEAD
using FUNCalendar.Models;
using System.Collections.Generic;
=======
>>>>>>> feature/menu
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
<<<<<<< HEAD
            /* 画面をDIコンテナに登録 */
=======

>>>>>>> feature/menu
            Container.RegisterTypeForNavigation<RootPage>();
            Container.RegisterTypeForNavigation<NavigationPage>();
            Container.RegisterTypeForNavigation<MenuPage>();
            Container.RegisterTypeForNavigation<CalendarPage>();
            Container.RegisterTypeForNavigation<WishListPage>();
            Container.RegisterTypeForNavigation<ToDoListPage>();
<<<<<<< HEAD
            Container.RegisterTypeForNavigation<WishListRegisterPage>();
            /* 共有のインスタンスをDIコンテナに登録 */
            var wishList = new WishList();
            Container.RegisterType<IWishList,WishList>(new ContainerControlledLifetimeManager());
=======
            Container.RegisterTypeForNavigation<HouseHoldAccountsPage>();
>>>>>>> feature/menu
        }
    }
}
