using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FUNCalendar.Models;

namespace FUNCalendar.ViewModels
{
    public class VMHouseholdAccountsStatisticItem
    {
        public string Bt { get; set; }
        public string Price { get; set; }

        public VMHouseholdAccountsStatisticItem(HouseholdAccountsStatisticItem item)
        {
            this.Bt = Enum.GetName(typeof(BalanceTypes), item.BalanceType);
            this.Price = string.Format("{0}円", item.Price);
        }
    }
}
