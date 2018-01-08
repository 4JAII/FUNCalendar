using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using OxyPlot;
using OxyPlot.Xamarin.Forms;
using OxyPlot.Series;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using FUNCalendar.Models;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Xamarin.Forms;
using Prism.Navigation;
using Prism.Services;
using System.Text.RegularExpressions;
using System.ComponentModel.DataAnnotations;

namespace FUNCalendar.ViewModels
{
    public class HouseholdaccountsRegisterPageViewModel : BindableBase,INavigationAware
    {
        private IHouseHoldAccounts _householdaccount;
        private INavigationService _navigationservice;

        /* 正しい遷移か確認するためのkey */
        public static readonly string InputKey = "InputKey";

        /* 遷移されたときの格納用変数 */
        public HouseholdaccountNavigationItem NavigatedItem { get; set; }

        /* 変更予定箇所 */
        public HouseholdaccountRangeItem[] RangeNames { get; private set; }
        public ReactiveProperty<DateTime> SelectedDate { get; private set; }    /* 日付を格納 */
        public ReactiveProperty<HouseholdaccountRangeItem> SelectedRange { get; private set; }   /* 範囲を格納 */
        /* 変更予定箇所END */

        [Required(ErrorMessage = "タイトルを入力してください")]
        public ReactiveProperty<string> Name { get; private set; } = new ReactiveProperty<string>();

        [Required(ErrorMessage ="金額を入力してください")]
        [RegularExpression("[0-9]+")]
        public ReactiveProperty<string> Price { get; private set; } = new ReactiveProperty<string>();

        [Required(ErrorMessage ="日付を指定してください")]
        public ReactiveProperty<DateTime> Date { get; private set; } = new ReactiveProperty<DateTime>();

        public ReactiveProperty<bool> CanRegister { get; private set; }
        public AsyncReactiveCommand RegisterHouseholdaccountsCommand { get; private set; }
        public AsyncReactiveCommand CancelCommand { get; private set; }

        public ReactiveProperty<Color> ErrorColor { get; private set; } = new ReactiveProperty<Color>();

        public HouseholdaccountsRegisterPageViewModel(IHouseHoldAccounts householdaccount, INavigationService navigationService)
        {
            this._householdaccount = householdaccount;
            this._navigationservice = navigationService;

            Name.SetValidateAttribute(() => this.Name);
            Price.SetValidateAttribute(() => this.Price);
            Date.SetValidateAttribute(() => this.Date);

            Date.Value = DateTime.Today;

            /* 変更予定箇所 */
            SelectedDate = new ReactiveProperty<DateTime>();
            SelectedDate.Value = DateTime.Today;
            SelectedRange = new ReactiveProperty<HouseholdaccountRangeItem>();

            RangeNames = new[]
            {
                new HouseholdaccountRangeItem
                {
                    RangeName = "統計:日単位",
                    R = Range.Day
                },
                new HouseholdaccountRangeItem
                {
                    RangeName = "統計:月単位" ,
                    R = Range.Month
                },
                new HouseholdaccountRangeItem
                {
                    RangeName = "統計:年単位",
                    R = Range.Year
                }
            };
            /* END */
            /* 未実装
            RegisterHouseholdaccountsCommand = CanRegister.ToAsyncReactiveCommand();
            RegisterHouseholdaccountsCommand.Subscribe(async () =>
            {
                if ()
                {
                    var item = new VMHouseHoldAccountsItem(Name.Value, Price.Value, Date.Value, "交通費", "飛行機", "財布", "支出");
                }
                else
                {

                }
            })
            */

            CanRegister = new[]
            {
                this.Name.ObserveHasErrors,
                this.Price.ObserveHasErrors,
                this.Date.ObserveHasErrors,
            }.CombineLatestValuesAreAllFalse().ToReactiveProperty<bool>();

            CanRegister.Subscribe(x =>
            {
                ErrorColor.Value = x ? Color.SkyBlue : Color.Gray;
            });

        }
        public void OnNavigatedFrom(NavigationParameters parameters)
        {
        }

        public void OnNavigatedTo(NavigationParameters parameters)
        {
            if (parameters.ContainsKey(InputKey))
            {
                NavigatedItem = (HouseholdaccountNavigationItem)parameters[InputKey];
                this.SelectedDate.Value = NavigatedItem.CurrentDate;
                this.SelectedRange.Value = (NavigatedItem.CurrentRange == Range.Day) ? RangeNames[0] :
                    (NavigatedItem.CurrentRange == Range.Month) ? RangeNames[1] :
                    (NavigatedItem.CurrentRange == Range.Year) ? RangeNames[2] : null;
            }
        }

        public void OnNavigatingTo(NavigationParameters parameters)
        {
        }
    }
}
