using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FUNCalendar.Models;

namespace FUNCalendar.ViewModels
{
    public class VMHouseholdAccountsLegendItem
    {
        public string ColorPath { get; set; }
        public string Label { get; set; }
        
        public VMHouseholdAccountsLegendItem(HouseholdAccountsLegendItem item)
        {
            this.ColorPath = item.ColorPath;
            this.Label = item.Label;
        }
    }
}
