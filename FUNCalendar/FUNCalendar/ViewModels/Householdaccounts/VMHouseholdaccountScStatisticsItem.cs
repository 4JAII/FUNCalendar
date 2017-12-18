using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FUNCalendar.Models;

namespace FUNCalendar.ViewModels
{
    public class VMHouseholdaccountScStatisticsItem
    {
        public string Bt { get; set; }
        public string Price { get; set; }
        public string Sc { get; set; }
        public string Ratio { get; set; }

        public VMHouseholdaccountScStatisticsItem(HouseholdaccountScStatisticsItem item)
        {
            this.Bt = Enum.GetName(typeof(Balancetype), item.Bt);
            this.Price = string.Format("{0}円", item .Price);
            this.Sc = Enum.GetName(typeof(SCategorys), item.Sc);
            this.Ratio = string.Format("{0}%", item.Ratio);
        }
    }
}
