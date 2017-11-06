using Xamarin.Forms;
using FUNCalendar.ViewModels;
using System;

namespace FUNCalendar.Views
{
    public partial class MenuPage : ContentPage
    {
        private RootPageViewModel ViewModel => this.BindingContext as RootPageViewModel;

        public MenuPage()
        {
            InitializeComponent();
        }

        private async void ListViewMenu_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            await this.ViewModel.PageChangeAsync(e.SelectedItem as ViewModels.MenuItem);
        }
    }
}
