using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FUNCalendar.Models
{
    public class HouseholdaccountScStatisticsItem :HouseholdaccountStatisticItem
    {
        public SCategorys Scategory { get; set; }
        public int Ratio { get; set; }

        public HouseholdaccountScStatisticsItem(BalanceTypes balancetype,SCategorys scategory, int price, int ratio):base(balancetype,price)
        {
            this.BalanceType = balancetype;
            this.Price = price;
            this.Scategory = scategory;
            this.Ratio = ratio;
        }
    }
}
