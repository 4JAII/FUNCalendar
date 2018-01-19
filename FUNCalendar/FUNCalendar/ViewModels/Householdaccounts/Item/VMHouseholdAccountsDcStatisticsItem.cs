using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FUNCalendar.Models;
using Prism.Mvvm;
using Prism.Navigation;

namespace FUNCalendar.ViewModels
{
    public class VMHouseholdAccountsDcStatisticsItem
    {
        public string Balancetype { get; set; }
        public string Scategory { set; get; }
        public string Dcategory { get; set; }
        public string Price { get; set; }
        public string Ratio { get; set; }

        public VMHouseholdAccountsDcStatisticsItem(HouseholdAccountsDcStatisticsItem item)
        {
            this.Balancetype = Enum.GetName(typeof(BalanceTypes), item.BalanceType);
            this.Price = string.Format("{0}円", item.Price);
            this.Scategory = Enum.GetName(typeof(SCategorys), item.Scategory);
            this.Dcategory = Enum.GetName(typeof(DCategorys), item.Dcategory);
            this.Ratio = string.Format("{0}%", item.Ratio);
        }

    }
}
