using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FUNCalendar.Models;

namespace FUNCalendar.ViewModels
{
    public class HouseholdaccountsStorageTypeItem
    {
        public string StorageTypeName { get; set; }
        public StorageTypes StorageTypeData { get; set; }

        public HouseholdaccountsStorageTypeItem(string name, StorageTypes storagetype)
        {
            this.StorageTypeName = name;
            this.StorageTypeData = storagetype;
        }
    }
}
