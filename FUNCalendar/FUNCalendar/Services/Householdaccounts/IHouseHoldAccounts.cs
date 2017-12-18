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
    public interface IHouseHoldAccounts:INotifyPropertyChanged
    {
        string SetTotalIncome { get; }
        string SetTotalOutgoing { get; }
        string SetDifference { get; }
        ObservableCollection<HouseholdaccountScStatisticsItem> SetSIncomes { get; }
        ObservableCollection<HouseholdaccountScStatisticsItem> SetSOutgoings { get; }
        ObservableCollection<HouseholdaccountPieSliceItem> SetPieSlice { get; }


        void SetAllStatics(Range r, DateTime date);
        void SetAllStaticsPie(Range r, Balancetype b, DateTime date);
        void AddHouseHoldAccountsItem(string name, int count, int price, DateTime date, DCategorys detailcategory, SCategorys summarycategory, StorageTypes storagetype, bool isoutgoings);
    }
}
