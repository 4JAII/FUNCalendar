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
        private IStorageService _storageService;

        private IWishList _wishList;

        public CalendarPageViewModel(IStorageService storageService)
        {
            this._storageService = storageService;

            canInitializeList.Subscribe(async _ =>
            {
                await _storageService.InitializeAsync();
                await _storageService.ReadFile();
                this._wishList = _storageService.WishList;
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
