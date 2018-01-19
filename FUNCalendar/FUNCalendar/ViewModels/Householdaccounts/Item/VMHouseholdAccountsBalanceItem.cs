using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FUNCalendar.Models;

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
            this.StorageType = Enum.GetName(typeof(StorageTypes), item.St);
            this.Price = string.Format("{0}円", item.Price);
            //this.Image = item.Image;
        }
        public VMHouseholdAccountsBalanceItem(/* int id,*/StorageTypes storagetype, int price/* , ImageSource image */)
        {
            this.StorageType = Enum.GetName(typeof(StorageTypes), storagetype);
            this.Price = String.Format("{0}円", price);
            //this.Image = image;
        }

        public static HouseholdAccountsBalanceItem ToHouseholdAccountsBalanceItem(VMHouseholdAccountsBalanceItem vmHouseholdAccountsBalanceItem)
        {
            int id = vmHouseholdAccountsBalanceItem.ID;
            StorageTypes storageTypes = (StorageTypes)Enum.Parse(typeof(StorageTypes), vmHouseholdAccountsBalanceItem.Image);
            int price = int.Parse(vmHouseholdAccountsBalanceItem.Price);
            return new HouseholdAccountsBalanceItem { ID = id, St = storageTypes, Price = price };
        }
    }
}
