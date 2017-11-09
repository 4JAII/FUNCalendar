using Prism.Mvvm;
using Prism.Navigation;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using Xamarin.Forms;

namespace FUNCalendar.ViewModels
{
    public class RootPageViewModel : BindableBase
    {
        public ObservableCollection<MenuItem> Menus { get; } = new ObservableCollection<MenuItem>
        {
            new MenuItem
            {
                Title="Calendar",
                PageName="CalendarPage",
                Image=ImageSource.FromFile("CalendarIcon.png")
            },
            new MenuItem
            {
                Title="ToDo",
                PageName="ToDoListPage",
                Image=ImageSource.FromFile("ToDoIcon.png")
            },
            new MenuItem
            {
                Title="WishList",
                PageName="WishListPage",
                Image=ImageSource.FromFile("WishListIcon.png")
            },
            new MenuItem
            {
                Title="家計簿",
                PageName="HouseHoldAccountsPage",
                Image=ImageSource.FromFile("HouseHoldAccountsIcon.png")
            }
        };

        private INavigationService NavigationService { get; }
        private bool isPresented;
        public bool IsPresented
        {
            get { return this.isPresented; }
            set { this.SetProperty(ref this.isPresented, value); }
        }

        public RootPageViewModel(INavigationService navigationService)
        {
            this.NavigationService = navigationService;
        }
        public async Task PageChangeAsync(MenuItem menuItem)
        {
            await this.NavigationService.NavigateAsync($"NavigationPage/{menuItem.PageName}");
            this.IsPresented = false;
        }
    }
}
