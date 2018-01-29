using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;

namespace FUNCalendar.Models
{
    public class HouseholdAccountsBalanceItem
    {
        public int ID { get; set; }
        public StorageTypes Storagetype { get; set; }
        public int Price { get; set; }
        public string Image { get; set; }

        public HouseholdAccountsBalanceItem() { }

        public HouseholdAccountsBalanceItem(StorageTypes storagetype, int price, string image)
        {
            ID = 0;
            this.Storagetype = storagetype;
            this.Price = price;
            this.Image = image;
        }

    }
}
