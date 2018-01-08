using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FUNCalendar.Models;

namespace FUNCalendar.ViewModels
{
    public class VMHouseholdaccountBalanceItem
    {
        public string StorageType { get; set; }
        public string Price { get; set; }
        //public ImageSource Image { get; set; }


        public VMHouseholdaccountBalanceItem(HouseholdaccountBalanceItem item)
        {
            this.StorageType = Enum.GetName(typeof(StorageTypes),item.St);
            this.Price = string.Format("{0}円", item.Price);
            //this.Image = item.Image;
        }
        public VMHouseholdaccountBalanceItem(StorageTypes storagetype, int price/* , ImageSource image */)
        {
            this.StorageType = Enum.GetName(typeof(StorageTypes), storagetype);
            this.Price = String.Format("{0}円", price);
            // this.Image = image;
        }
    }
}
