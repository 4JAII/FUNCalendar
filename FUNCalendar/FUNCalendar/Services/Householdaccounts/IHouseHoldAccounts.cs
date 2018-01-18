using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using Prism.Mvvm;
using Xamarin.Forms;
using System.Reactive.Linq;
using System.ComponentModel;

namespace FUNCalendar.Models
{
    public interface IHouseholdAccounts : INotifyPropertyChanged
    {
        string TotalIncome { get; }
        string TotalOutgoing { get; }
        string Difference { get; }
        string TotalBalance { get; }
        string SCategoryTotal { get; }
        HouseholdAccountsItem SelectedItem { get; }
        ObservableCollection<HouseholdAccountsScStatisticsItem> SIncomes { get; }
        ObservableCollection<HouseholdAccountsScStatisticsItem> SOutgoings { get; }
        ObservableCollection<HouseholdAccountsPieSliceItem> PieSlice { get; }
        ObservableCollection<HouseholdAccountsBalanceItem> Balances { get; }
        ObservableCollection<HouseholdAccountsLegendItem> Legend { get; }
        ObservableCollection<HouseholdAccountsDcStatisticsItem> ScategoryItems { get; }
        ObservableCollection<HouseholdAccountsItem> DisplayHouseholdaccountList { get; }

        void SetAllStatics(Range r, DateTime date);
        void SetAllStaticsPie(Range r, BalanceTypes b, DateTime date);
        void SetAllHistory(Range r, DateTime date);
        void SetSCategoryStatics(Range r, BalanceTypes b, DateTime date, SCategorys sc);
        void SetSCategoryStatisticsPie(Range r, DateTime date, SCategorys sc);
        void SetDCategoryHistory(Range r, DateTime date, DCategorys dc);
        void AddHouseHoldAccountsItem(string name, int price, DateTime date, DCategorys detailcategory, SCategorys summarycategory, StorageTypes storagetype, bool isoutgoings);
        void AddHouseHoldAccountsItem(HouseholdAccountsItem item);
        void SetHouseholdaccountsItem(HouseholdAccountsItem item);
        void EditHouseholdaccountsItem(HouseholdAccountsItem deleteItem, HouseholdAccountsItem additem);
        void SetBalance();
        void IncrementBalancePrice(StorageTypes st, int price);
        void EditHouseholdaccountBalance(StorageTypes st, int price);
        int ScToDcStart(SCategorys sc);
        int ScToDcEnd(SCategorys sc);
    }
}
