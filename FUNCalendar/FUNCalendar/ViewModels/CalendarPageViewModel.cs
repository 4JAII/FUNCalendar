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
        private static ReactiveProperty<bool> canInitialize= new ReactiveProperty<bool>();
        private IStorageService _storageService;

        private IWishList _wishList;

        public CalendarPageViewModel(IWishList wishList,IStorageService storageService)
        {
            this._storageService = storageService;
            this._wishList = wishList;
            canInitialize.Subscribe(async _ =>
            {
                await _storageService.InitializeAsync(this._wishList);
                await _storageService.ReadFile();
                
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
    }
}
