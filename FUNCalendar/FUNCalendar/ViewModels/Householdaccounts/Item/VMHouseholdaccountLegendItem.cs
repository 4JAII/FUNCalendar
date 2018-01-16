using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FUNCalendar.Models;

namespace FUNCalendar.ViewModels
{
    public class VMHouseholdaccountLegendItem
    {
        public string ColorPath { get; set; }
        public string Label { get; set; }
        
        public VMHouseholdaccountLegendItem(HouseholdaccountLegendItem item)
        {
            this.ColorPath = item.ColorPath;
            this.Label = item.Label;
        }
    }
}
