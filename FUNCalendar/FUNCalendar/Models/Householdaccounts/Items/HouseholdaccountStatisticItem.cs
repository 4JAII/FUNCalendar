using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FUNCalendar.Models
{
    public class HouseholdaccountStatisticItem
    {
        public BalanceTypes BalanceType { get; set; }
        public int Price { get; set; }

        public HouseholdaccountStatisticItem(BalanceTypes balanctype,int price)
        {
            this.BalanceType = balanctype;
            this.Price = price;
        }
    }
}
