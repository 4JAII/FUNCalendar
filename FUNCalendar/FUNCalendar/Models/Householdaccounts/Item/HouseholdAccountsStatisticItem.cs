using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FUNCalendar.Models
{
    public class HouseholdAccountsStatisticItem
    {
        public BalanceTypes BalanceType { get; set; }
        public int Price { get; set; }

        public HouseholdAccountsStatisticItem(BalanceTypes balanctype,int price)
        {
            this.BalanceType = balanctype;
            this.Price = price;
        }
    }
}
