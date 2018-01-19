using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FUNCalendar.Models
{
    public class HouseholdAccountsPieSliceItem
    {
        public string Label { get; set; }
        public int Price { get; set; }
        public string ColorPath { get; set; }

        public HouseholdAccountsPieSliceItem(string label, int price, string color)
        {
            this.Label = label;
            this.Price = price;
            this.ColorPath = color;
        }
    }
}
