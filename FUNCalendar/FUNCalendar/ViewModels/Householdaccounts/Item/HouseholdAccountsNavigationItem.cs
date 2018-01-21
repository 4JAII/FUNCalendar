using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FUNCalendar.Models;


namespace FUNCalendar.ViewModels
{
    public class HouseholdAccountsNavigationItem
    {

        public BalanceTypes CurrentBalanceType { get; set; }
        public SCategorys CurrentSCategory { get; set; }
        public DCategorys CurrentDCategory { get; set; }
        public StorageTypes CurrentStoragetype { get; set; }
        public string Price { get; set; }
        public DateTime CurrentDate { get; set; }
        public Range CurrentRange { get; set; }


        public HouseholdAccountsNavigationItem(DateTime date, Range range)
        {
            this.CurrentRange = range;
            this.CurrentDate = date;
        }

        public HouseholdAccountsNavigationItem(BalanceTypes balancetype, SCategorys scategory, DateTime date, Range range)
        {
            this.CurrentBalanceType = balancetype;
            this.CurrentSCategory = scategory;
            this.CurrentDate = date;
            this.CurrentRange = range;
        }

        public HouseholdAccountsNavigationItem(BalanceTypes balancetype, SCategorys scategory, DCategorys dcategory, DateTime date, Range range)
        {
            this.CurrentBalanceType = balancetype;
            this.CurrentSCategory = scategory;
            this.CurrentDCategory = dcategory;
            this.CurrentDate = date;
            this.CurrentRange = range;
        }

        public HouseholdAccountsNavigationItem(StorageTypes storagetype,string price, DateTime date, Range range)
        {
            this.CurrentStoragetype = storagetype;
            this.Price = price;
            this.CurrentDate = date;
            this.CurrentRange = range;
        }

    }
}
