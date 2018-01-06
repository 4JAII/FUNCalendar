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

        /* デバッグ用 */
        //public ReactiveProperty<string> DebugText { get; private set; } = new ReactiveProperty<string>("まだボタンは押されていません");
        public ReactiveProperty<int> ToDoListHeight { get; private set; } = new ReactiveProperty<int>(0);
        public ReactiveProperty<int> WishListHeight { get; private set; } = new ReactiveProperty<int>(0);
        public ReactiveProperty<int> HouseHoldAccountsListHeight { get; private set; } = new ReactiveProperty<int>(0);

        /* 廃棄 */
        private CompositeDisposable Disposable { get; } = new CompositeDisposable();

        public CalendarDetailPageViewModel(ICalendar calendar, INavigationService navigationService, IPageDialogService pageDialogService)
        {
            /* コンストラクタインジェクションされたインスタンスを保持 */
            this._calendar = calendar;
            this._navigationService = navigationService;
            this._pageDialogService = pageDialogService;

            BackCommand = new ReactiveCommand();
            ToDoListOpenCloseCommand = new ReactiveCommand();
            WishListOpenCloseCommand = new ReactiveCommand();
            HouseHoldAccountsListOpenCloseCommand = new ReactiveCommand();

            /* 戻る処理 */
            BackCommand.Subscribe(async () =>
            {
                await _navigationService.NavigateAsync($"/RootPage/NavigationPage/CalendarPage");
            });

            /* List開閉処理 */
            ToDoListOpenCloseCommand.Subscribe(() =>
            {
                if (ToDoListHeight.Value > 0)
                {
                    ToDoListHeight.Value = 0;
                }
                else
                {
                    ToDoListHeight.Value = 100;
                }
            });

            WishListOpenCloseCommand.Subscribe(() =>
            {
                if (WishListHeight.Value > 0)
                {
                    WishListHeight.Value = 0;
                }
                else
                {
                    WishListHeight.Value = 100;
                }
            });

            HouseHoldAccountsListOpenCloseCommand.Subscribe(() =>
            {
                if (HouseHoldAccountsListHeight.Value > 0)
                {
                    HouseHoldAccountsListHeight.Value = 0;
                }
                else
                {
                    HouseHoldAccountsListHeight.Value = 100;
                }
            });
        }

        public void OnNavigatedFrom(NavigationParameters parameters)
        {
            
        }

        public void OnNavigatedTo(NavigationParameters parameters)
        {
            VMDate vmDate = new VMDate(_calendar.DisplayDate);
            DateData.Value = vmDate.DateData;
            DisplayWishList = new ObservableCollection<WishItem>(vmDate.WishList).ToReadOnlyReactiveCollection(x => new VMWishItem(x)).AddTo(Disposable);
        }

        public void OnNavigatingTo(NavigationParameters parameters)
        {
            
        }
    }
}
