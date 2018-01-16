﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FUNCalendar.Models;

namespace FUNCalendar.ViewModels
{
    public class HouseholdaccountsDcategoryItem
    {
        public string DcategoryName { get; set; }
        public DCategorys DcategoryData { get; set; }

        public HouseholdaccountsDcategoryItem(string name, DCategorys dc)
        {
            this.DcategoryName = name;
            this.DcategoryData = dc;
        }
    }
}
