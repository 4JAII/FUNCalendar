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
    public interface IHouseHoldAccounts : INotifyPropertyChanged
    {
        string TotalIncome { get; }
        string TotalOutgoing { get; }
        string Difference { get; }
        string TotalBalance { get; }
        string SCategoryTotal { get; }
        HouseHoldAccountsItem SelectedItem { get; }
        ObservableCollection<HouseholdaccountScStatisticsItem> SIncomes { get; }
        ObservableCollection<HouseholdaccountScStatisticsItem> SOutgoings { get; }
        ObservableCollection<HouseholdaccountPieSliceItem> PieSlice { get; }
        ObservableCollection<HouseholdaccountBalanceItem> Balances { get; }
        ObservableCollection<HouseholdaccountLegendItem> Legend { get; }
        ObservableCollection<HouseholdaccountDcStatisticsItem> ScategoryItems { get; }
        ObservableCollection<HouseHoldAccountsItem> DisplayHouseholdaccountList { get; }

        void SetAllStatics(Range r, DateTime date);
        void SetAllStaticsPie(Range r, BalanceTypes b, DateTime date);
        void SetAllHistory(Range r, DateTime date);
        void SetSCategoryStatics(Range r, BalanceTypes b, DateTime date, SCategorys sc);
        void SetSCategoryStatisticsPie(Range r, DateTime date, SCategorys sc);
        void SetDCategoryHistory(Range r, DateTime date, DCategorys dc);
        void AddHouseHoldAccountsItem(string name, int price, DateTime date, DCategorys detailcategory, SCategorys summarycategory, StorageTypes storagetype, bool isoutgoings);
        void AddHouseHoldAccountsItem(HouseHoldAccountsItem item);
        void SetHouseholdaccountsItem(HouseHoldAccountsItem item);
        void EditHouseholdaccountsItem(HouseHoldAccountsItem deleteItem, HouseHoldAccountsItem additem);
        void SetBalance();
        void IncrementBalancePrice(StorageTypes st, int price);
        void EditHouseholdaccountBalance(StorageTypes st, int price);
        int ScToDcStart(SCategorys sc);
        int ScToDcEnd(SCategorys sc);
    }
}
