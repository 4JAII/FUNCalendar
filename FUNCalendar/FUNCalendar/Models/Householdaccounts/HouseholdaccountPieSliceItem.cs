using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FUNCalendar.Models
{
    public class HouseholdaccountPieSliceItem
    {
        public string Label { get; set; }
        public int Price { get; set; }
        public string ColorPath { get; set; }

        public HouseholdaccountPieSliceItem(string label, int price, string color)
        {
            this.Label = label;
            this.Price = price;
            this.ColorPath = color;
        }
    }
}
