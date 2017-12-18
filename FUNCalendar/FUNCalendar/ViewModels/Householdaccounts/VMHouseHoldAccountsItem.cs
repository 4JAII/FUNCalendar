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
        public int ID { get; private set; }
        public string Name { get; private set; }
        public string Count { get; private set; }
        public string Price { get; private set; }
        public string Date { get; private set; }
        public string Summarycategory { get; private set; }
        public string Detailcategory { get; private set; }
        public string Storagetype { get; private set; }
        public string IsOutGoings { get; private set; }

        /* コンストラクタ(HouseHoldAccountsItem To VMHouseHoldAccountsItem) */
        public VMHouseHoldAccountsItem(HouseHoldAccountsItem item)
        {
            this.ID = item.ID;
            this.Name = item.Name;
            this.Count = string.Format("{0}個", item.Count);
            this.Price = string.Format("{0}円", item.Price);
            this.Date = item.Date.ToString("yyyy/mm/dd");
            this.Summarycategory = Enum.GetName(typeof(SCategorys), item.SummaryCategory);
            this.Detailcategory = Enum.GetName(typeof(DCategorys), item.DetailCategory);
            this.Storagetype = Enum.GetName(typeof(StorageTypes), item.StorageType);
            this.IsOutGoings = item.IsOutGoings ? "支出" : "収入";
        }

        /* 編集用 */
        public VMHouseHoldAccountsItem(int id,string name,string count,string price,DateTime date,string summarycategory,string detailcategory,string storagetype,string isoutogoings)
        {
            this.ID = id;
            this.Name = name;
            this.Count = count;
            this.Price = price;
            this.Date = date.Date.ToString("yyyy/mm/dd");
            this.Summarycategory = summarycategory;
            this.Detailcategory = detailcategory;
            this.Storagetype = storagetype;
            this.IsOutGoings = isoutogoings;
        }
    }
}
