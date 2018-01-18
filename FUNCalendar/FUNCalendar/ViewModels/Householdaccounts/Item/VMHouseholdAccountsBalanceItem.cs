using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FUNCalendar.Models;
using System.Text.RegularExpressions;

namespace FUNCalendar.ViewModels
{
    public class VMHouseholdAccountsBalanceItem
    {
        public int ID { get; set; }
        public string StorageType { get; set; }
        public string Price { get; set; }
        public string Image { get; set; }


        public VMHouseholdAccountsBalanceItem(HouseholdAccountsBalanceItem item)
        {
            this.ID = item.ID;
            this.StorageType = Enum.GetName(typeof(StorageTypes), item.Storagetype);
            this.Price = string.Format("{0}円", item.Price);
            //this.Image = item.Image;
        }
        public VMHouseholdAccountsBalanceItem(int id, StorageTypes storagetype, string price/* , ImageSource image */)
        {
            this.ID = id;
            this.StorageType = Enum.GetName(typeof(StorageTypes), storagetype);
            this.Price = String.Format("{0}円", price);
            //this.Image = image;
        }
        public VMHouseholdAccountsBalanceItem() { }

        public static HouseholdAccountsBalanceItem ToHouseholdAccountsBalanceItem(VMHouseholdAccountsBalanceItem item)
        {
            Regex re = new Regex("[^0-9]");
            var id = item.ID;
            var price = int.Parse(re.Replace(item.Price, ""));
            var storagetype = (StorageTypes)Enum.Parse(typeof(StorageTypes), item.StorageType);
            return new HouseholdAccountsBalanceItem() { ID = id, Price = price, Storagetype = storagetype };
        }

        /*
        public static HouseholdAccountsBalanceItem ToHouseholdAccountsBalanceItem(VMHouseholdAccountsBalanceItem vmHouseholdAccountsBalanceItem)
        {
            int id = vmHouseholdAccountsBalanceItem.ID;
            StorageTypes storageTypes = (StorageTypes)Enum.Parse(typeof(StorageTypes), vmHouseholdAccountsBalanceItem.Image);
            int price = int.Parse(vmHouseholdAccountsBalanceItem.Price);
            return new HouseholdAccountsBalanceItem { ID = id, St = storageTypes, Price = price };
        }*/
    }
}
