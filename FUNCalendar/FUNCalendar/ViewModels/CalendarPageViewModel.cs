using Prism.Mvvm;
using Prism.Navigation;
using Reactive.Bindings;
using FUNCalendar.Models;
using FUNCalendar.Services;
using System;

namespace FUNCalendar.ViewModels
{
    public class CalendarPageViewModel : BindableBase,INavigationAware
    {
        /* 全てのリストを初期化 */
        private static ReactiveProperty<bool> canInitializeList= new ReactiveProperty<bool>();
        private LocalStorage localStorage = new LocalStorage();

        private IWishList _wishList;

        public CalendarPageViewModel(IWishList wishList)
        {
            this._wishList = wishList;

            canInitializeList.Subscribe(async _ =>
            {
                _wishList.InitializeList(await localStorage.ReadFile());
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
    }
}
