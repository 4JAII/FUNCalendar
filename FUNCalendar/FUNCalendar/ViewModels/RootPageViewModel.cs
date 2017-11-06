using Prism.Mvvm;
using Prism.Navigation;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace FUNCalendar.ViewModels
{
    public class RootPageViewModel : BindableBase
    {
        public ObservableCollection<MenuItem> Menus { get; } = new ObservableCollection<MenuItem>
        {
            new MenuItem
            {
                Title="Calendar",
                PageName="CalendarPage"
            },
            new MenuItem
            {
                Title="ToDo",
                PageName="ToDoListPage"
            },
                        new MenuItem
            {
                Title="WishList",
                PageName="WishListPage"
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
