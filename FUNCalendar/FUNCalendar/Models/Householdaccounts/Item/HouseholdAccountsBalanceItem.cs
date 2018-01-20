using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;

namespace FUNCalendar.Models
{
    //[Table("HouseholdAccountsBalanceItem")]
    public class HouseholdAccountsBalanceItem
    {
        //[PrimaryKey, AutoIncrement]
        public int ID { get; set; }
        public  StorageTypes Storagetype { get; set; }
        public int Price { get; set; }
        //public Imagesource Image { get; set; }

        public HouseholdAccountsBalanceItem() { }

        public HouseholdAccountsBalanceItem(int id, StorageTypes storagetype, int price /* stirng image */)
        {
            this.ID = id;
            this.Storagetype = storagetype;
            this.Price = price;
            //ImageSource = image;
        }

    }
}
