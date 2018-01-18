using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FUNCalendar.Models;

namespace FUNCalendar.ViewModels
{
    public class HouseholdAccountsSCategoryItem
    {
        public string ScategoryName { get; set; }
        public SCategorys ScategoryData { get; set; }

        public HouseholdAccountsSCategoryItem(string name, SCategorys scategory)
        {
            this.ScategoryName = name;
            this.ScategoryData = scategory;
        }
    }
}
