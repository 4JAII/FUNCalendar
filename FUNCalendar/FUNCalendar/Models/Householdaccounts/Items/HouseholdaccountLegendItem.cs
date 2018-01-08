using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace FUNCalendar.Models
{
    public class HouseholdaccountLegendItem
    {
        public string ColorPath { get; set; }
        public string Label { get; set; }

        public HouseholdaccountLegendItem(string color, string label)
        {
            this.ColorPath = color;
            this.Label = label;
        }

    }
}
