using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FUNCalendar.Models;

namespace FUNCalendar.ViewModels
{
    public class HouseholdaccountsSCategoryItem
    {
        public string ScategoryName { get; set; }
        public SCategorys ScategoryData { get; set; }

        public HouseholdaccountsSCategoryItem(string name, SCategorys scategory)
        {
            this.ScategoryName = name;
            this.ScategoryData = scategory;
        }
    }
}
