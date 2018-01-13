using Prism.Commands;
using Prism.Mvvm;
using Prism.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using FUNCalendar.Models;
using FUNCalendar.Views;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using Reactive.Bindings.Binding;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Xamarin.Forms;
using Microsoft.Practices.Unity;
using Prism.Navigation;
using System.Text.RegularExpressions;
using System.Reactive.Disposables;
using System.Collections.ObjectModel;

namespace FUNCalendar.ViewModels
{
    public class CalendarDetailPageViewModel : BindableBase, INavigationAware
    {
        private ICalendar _calendar;
        private IWishList _wishList;
        private INavigationService _navigationService;
        private IPageDialogService _pageDialogService;

        /* Date詳細画面用 */
        public ReactiveProperty<DateTime> DateData { get; private set; } = new ReactiveProperty<DateTime>();
        public ReadOnlyReactiveCollection<VMWishItem> DisplayWishList { get; private set; }
        //public ReadOnlyReactiveCollection<VMToDoItem> DisplayToDoList { get; private set; }
        //public ReadOnlyReactiveCollection<VMHouseHoldAccountsItem> DisplayHouseHoldAccountsList { get; private set; }

        /* Command関連 */
        public ReactiveCommand BackCommand { get; private set; }
        public ReactiveCommand ToDoListOpenCloseCommand { get; private set; }
        public ReactiveCommand WishListOpenCloseCommand { get; private set; }
        public ReactiveCommand HouseHoldAccountsListOpenCloseCommand { get; private set; }

        /* 廃棄 */
        private CompositeDisposable Disposable { get; } = new CompositeDisposable();

        public CalendarDetailPageViewModel(ICalendar calendar,IWishList wishList, INavigationService navigationService, IPageDialogService pageDialogService)
        {
            /* コンストラクタインジェクションされたインスタンスを保持 */
            this._calendar = calendar;
            this._wishList = wishList;
            this._navigationService = navigationService;
            this._pageDialogService = pageDialogService;

            BackCommand = new ReactiveCommand();
            ToDoListOpenCloseCommand = new ReactiveCommand();
            WishListOpenCloseCommand = new ReactiveCommand();
            HouseHoldAccountsListOpenCloseCommand = new ReactiveCommand();

            DisplayWishList = _wishList.WishListForCalendar.ToReadOnlyReactiveCollection(x => new VMWishItem(x)).AddTo(Disposable);
            //DisplayWishList = _wishList.WishListForCalendar.ToReadOnlyReactiveCollection(x => new VMWishItem(x)).AddTo(Disposable);
            //DisplayWishList = _wishList.WishListForCalendar.ToReadOnlyReactiveCollection(x => new VMWishItem(x)).AddTo(Disposable);

            /* DatePicker */
            DateData.Subscribe( _ =>
            {
                _wishList.ClearWishListForCalendar();
                //_todoList.ClearToDoListForCalendar();
                //_houseHoldAccountsList.ClearHouseHoldAcountsListForCalendar();
            });

            /* 戻る処理 */
            BackCommand.Subscribe(async () =>
            {
                await _navigationService.NavigateAsync($"/RootPage/NavigationPage/CalendarPage");
            });

            /* List開閉処理 */
            ToDoListOpenCloseCommand.Subscribe(() =>
            {
                /*
                if (_todoList.ToDoListForCalendar.Any())
                {
                    _todoList.ClearToDoListForCalendar();
                }
                else
                {
                    _todoList.SetToDoListForCalendar(DateData.Value);
                }
                */
            });

            WishListOpenCloseCommand.Subscribe(() =>
            {
                if (_wishList.WishListForCalendar.Any())
                {
                    _wishList.ClearWishListForCalendar();
                }
                else
                {
                    _wishList.SetWishListForCalendar(DateData.Value);
                }
            });

            HouseHoldAccountsListOpenCloseCommand.Subscribe(() =>
            {
                /*
                if (_houseHoldAccountsList.HouseHoldAccountsListForCalendar.Any())
                {
                    _houseHoldAccountsList.ClearHouseHoldAccountsListForCalendar();
                }
                else
                {
                    _houseHoldAccountsList.SetHouseHoldAccountsListForCalendar(DateData.Value);
                }
                */
            });
        }

        public void OnNavigatedFrom(NavigationParameters parameters)
        {
            
        }

        public void OnNavigatedTo(NavigationParameters parameters)
        {
            DateData.Value = _calendar.DisplayDate.DateData;
        }

        public void OnNavigatingTo(NavigationParameters parameters)
        {
            
        }
    }
}
