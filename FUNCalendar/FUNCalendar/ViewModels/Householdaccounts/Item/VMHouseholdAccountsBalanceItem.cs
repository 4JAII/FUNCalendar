using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FUNCalendar.Models;
using System.Text.RegularExpressions;
using Xamarin.Forms;


namespace FUNCalendar.ViewModels
{
    public class VMHouseholdAccountsBalanceItem
    {
        public int ID { get; set; }
        public string StorageType { get; set; }
        public string Price { get; set; }
        public ImageSource Image { get; set; }


        public VMHouseholdAccountsBalanceItem(HouseholdAccountsBalanceItem item)
        {
            ID = 0;
            this.StorageType = Enum.GetName(typeof(StorageTypes), item.Storagetype);
            this.Price = string.Format("{0}円", item.Price);
            this.Image = ImageSource.FromFile(item.Image);
        }
        public VMHouseholdAccountsBalanceItem(StorageTypes storagetype, string price, ImageSource image)
        {
            ID = 0;
            this.StorageType = Enum.GetName(typeof(StorageTypes), storagetype);
            this.Price = String.Format("{0}円", price);
            this.Image = image;
        }
        public VMHouseholdAccountsBalanceItem() { }

        public static HouseholdAccountsBalanceItem ToHouseholdAccountsBalanceItem(VMHouseholdAccountsBalanceItem item)
        {
            return new HouseholdAccountsBalanceItem();
        }
    }
}
