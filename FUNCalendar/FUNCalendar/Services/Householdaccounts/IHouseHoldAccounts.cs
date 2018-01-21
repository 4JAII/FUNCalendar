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
        HouseholdAccountsBalanceItem SelectedBalanceItem { get; }
        ObservableCollection<HouseholdAccountsScStatisticsItem> SIncomes { get; }
        ObservableCollection<HouseholdAccountsScStatisticsItem> SOutgoings { get; }
        ObservableCollection<HouseholdAccountsPieSliceItem> PieSlice { get; }
        ObservableCollection<HouseholdAccountsBalanceItem> Balances { get; }
        ObservableCollection<HouseholdAccountsLegendItem> Legend { get; }
        ObservableCollection<HouseholdAccountsDcStatisticsItem> ScategoryItems { get; }
        ObservableCollection<HouseholdAccountsItem> DisplayHouseholdaccountList { get; }

        void UpdateList(List<HouseholdAccountsItem> list);
        //void UpdateBalanceList(List<HouseholdAccountsBalanceItem> list);
        void SetAllStatics(Range r, DateTime date);
        void SetAllStaticsPie(Range r, BalanceTypes b, DateTime date);
        void SetAllHistory(Range r, DateTime date);
        void SetSCategoryStatics(Range r, BalanceTypes b, DateTime date, SCategorys sc);
        void SetSCategoryStatisticsPie(Range r, DateTime date, SCategorys sc);
        void SetDCategoryHistory(Range r, DateTime date, DCategorys dc);
        //void AddHouseholdAccountsItem(string name, int price, DateTime date, DCategorys detailcategory, SCategorys summarycategory, StorageTypes storagetype, bool isoutgoings);
        void AddHouseholdAccountsItem(HouseholdAccountsItem item);
        void SetHouseholdAccountsItem(HouseholdAccountsItem item);
        void SetHouseholdAccountsBalanceItem(HouseholdAccountsBalanceItem item); 
        void EditHouseholdAccountsItem(HouseholdAccountsItem deleteItem, HouseholdAccountsItem additem);
        void RemoveHouseholdAccountsItem(HouseholdAccountsItem deleteitem);
        void SetBalance();
        void EditHouseholdAccountsBalanceItem(HouseholdAccountsBalanceItem deleteitem, HouseholdAccountsBalanceItem additem);
        void EditHouseholdAccountsBalanceItem(HouseholdAccountsBalanceItem item, int price, bool isoutgoing, bool isincrement);
        int ScToDcStart(SCategorys sc);
        int ScToDcEnd(SCategorys sc);
    }
}
