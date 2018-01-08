using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FUNCalendar.Models;

namespace FUNCalendar.ViewModels
{
    public class VMHouseHoldAccountsItem
    {
        public string Name { get; private set; }
        public string Price { get; private set; }
        public string Date { get; private set; }
        public string SCategory { get; private set; }
        public string DCategory { get; private set; }
        public string Storagetype { get; private set; }
        public string CategoryData { get; private set; }
        public string IsOutGoings { get; private set; }

        /* コンストラクタ(HouseHoldAccountsItem To VMHouseHoldAccountsItem) */
        public VMHouseHoldAccountsItem(HouseHoldAccountsItem item)
        {
            this.Name = item.Name;
            this.Price = string.Format("{0}円", item.Price);
            this.Date = item.Date.ToString("yyyy/mm/dd");
            this.SCategory = Enum.GetName(typeof(SCategorys), item.SCategory);
            this.DCategory = Enum.GetName(typeof(DCategorys), item.DCategory);
            this.Storagetype = Enum.GetName(typeof(StorageTypes), item.StorageType);
            this.IsOutGoings = item.IsOutGoings ? "支出" : "収入";
            this.CategoryData = string.Format("{0}>{1}", item.SCategory, item.DCategory);
        }

        /* 編集用 */
        public VMHouseHoldAccountsItem(string name, string price, DateTime date, string scategory, string dcategory, string storagetype, string isoutogoings)
        {
            this.Name = name;
            this.Price = price;
            this.Date = date.Date.ToString("yyyy/mm/dd");
            this.SCategory = scategory;
            this.DCategory = dcategory;
            this.Storagetype = storagetype;
            this.IsOutGoings = isoutogoings;
            this.CategoryData = string.Format("{0}>{1}", scategory, dcategory);
        }
    }
}
