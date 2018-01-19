using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FUNCalendar.Models;

namespace FUNCalendar.ViewModels
{
    public class HouseholdAccountsStorageTypeItem
    {
        public string StorageTypeName { get; set; }
        public StorageTypes StorageTypeData { get; set; }

        public HouseholdAccountsStorageTypeItem(string name, StorageTypes storagetype)
        {
            this.StorageTypeName = name;
            this.StorageTypeData = storagetype;
        }
    }
}
