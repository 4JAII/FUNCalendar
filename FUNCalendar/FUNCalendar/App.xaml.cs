using Prism.Unity;
using FUNCalendar.Views;
using FUNCalendar
using Xamarin.Forms;

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
            Container.RegisterType<I>()

            Container.RegisterTypeForNavigation<RootPage>();
            Container.RegisterTypeForNavigation<NavigationPage>();
            Container.RegisterTypeForNavigation<MenuPage>();
            Container.RegisterTypeForNavigation<CalendarPage>();
            Container.RegisterTypeForNavigation<WishListPage>();
            Container.RegisterTypeForNavigation<ToDoListPage>();
        }
    }
}
