using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FUNCalendar.Models
{
    public class HouseholdaccountScStatisticsItem :HouseholdaccountStatisticItem
    {
        public SCategorys Sc { get; set; }
        public int Ratio { get; set; }

        public HouseholdaccountScStatisticsItem(Balancetype bt,SCategorys sc, int price, int ratio):base(bt,price)
        {
            this.Bt = bt;
            this.Price = price;
            this.Sc = sc;
            this.Ratio = ratio;
        }
    }
}
