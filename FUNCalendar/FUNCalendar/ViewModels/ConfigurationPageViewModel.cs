using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.Services;
using FUNCalendar.Models;
using FUNCalendar.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using System.Reactive.Linq;
using System.Reactive.Disposables;

namespace FUNCalendar.ViewModels
{
    public class ConfigurationPageViewModel : BindableBase , INavigationAware
    {
        private IStorageService _storageService;
        private INavigationService _navigationService;
        private IPageDialogService _pageDialogService;

        public ReactiveProperty<string> Username { get; private set; }
        public ReactiveProperty<string> Password { get; private set; }
        public ReactiveProperty<bool> IsEnableRemoteStorage { get; private set; }

        public ReactiveProperty<bool> CanExecute { get; private set; }
        public ReactiveCommand SaveConfigCommand { get; private set; }

        private CompositeDisposable disposable = new CompositeDisposable();

        public ConfigurationPageViewModel(IStorageService storageService, INavigationService navigationService, IPageDialogService pageDialogService)
        {
            this._storageService = storageService;
            this._navigationService = navigationService;
            this._pageDialogService = pageDialogService;
            Username = _storageService.Config.ObserveProperty(x=>x.Username).ToReactiveProperty().AddTo(disposable);
            Password = _storageService.Config.ObserveProperty(x=>x.Password).ToReactiveProperty().AddTo(disposable);
            IsEnableRemoteStorage = _storageService.Config.ObserveProperty(x=>x.IsEnableRemoteStorage).ToReactiveProperty().AddTo(disposable);
            CanExecute = new ReactiveProperty<bool> { Value = false };
            SaveConfigCommand = new ReactiveCommand();

            Username.Subscribe(_ => CanExecute.Value = true);
            Password.Subscribe(_ => CanExecute.Value = true);
            IsEnableRemoteStorage.Subscribe(_ => CanExecute.Value = true);
            SaveConfigCommand.Subscribe(async (_) =>
            {
                await _storageService.SetConfig(IsEnableRemoteStorage.Value, Username.Value, Password.Value);
                CanExecute.Value = false;
            });
        }

        public void OnNavigatedFrom(NavigationParameters parameters)
        {
            disposable.Dispose();
        }

        public void OnNavigatedTo(NavigationParameters parameters)
        {
            
        }

        public void OnNavigatingTo(NavigationParameters parameters)
        {
           
        }
    }
}
