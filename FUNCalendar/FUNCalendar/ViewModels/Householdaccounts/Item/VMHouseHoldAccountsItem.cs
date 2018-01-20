using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FUNCalendar.Models;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace FUNCalendar.ViewModels
{
    [JsonObject("household_accounts_item")]
    public class VMHouseholdAccountsItem
    {
        [JsonProperty("id")]
        public int ID { get; private set; }
        [JsonProperty("name")]
        public string Name { get; private set; }
        [JsonProperty("price")]
        public string Price { get; private set; }
        [JsonProperty("date")]
        public string Date { get; private set; }
        [JsonProperty("summary_category")]
        public string SCategory { get; private set; }
        [JsonProperty("detail_category")]
        public string DCategory { get; private set; }
        [JsonProperty("storage_type")]
        public string Storagetype { get; private set; }
        [JsonIgnore]
        public string CategoryData { get; private set; }
        [JsonProperty("is_out_goings")]
        public string IsOutGoings { get; private set; }

        public VMHouseholdAccountsItem() { }

        /* コンストラクタ(HouseHoldAccountsItem To VMHouseHoldAccountsItem) */
        public VMHouseholdAccountsItem(HouseholdAccountsItem item)
        {
            this.ID = item.ID;
            this.Name = item.Name;
            this.Price = string.Format("{0}円", item.Price);
            this.Date = item.Date.ToString("yyyy/MM/dd");
            this.SCategory = Enum.GetName(typeof(SCategorys), item.SCategory);
            this.DCategory = Enum.GetName(typeof(DCategorys), item.DCategory);
            this.Storagetype = Enum.GetName(typeof(StorageTypes), item.StorageType);
            this.IsOutGoings = item.IsOutGoings ? "支出" : "収入";
            this.CategoryData = string.Format("{0}>{1}>{2}",IsOutGoings, item.SCategory, item.DCategory);
        }

        /* 編集用 */
        public VMHouseholdAccountsItem(int id,string name, string price, DateTime date, string scategory, string dcategory, string storagetype, string isoutogoings)
        {
            this.ID = id;
            this.Name = name;
            this.Price = price;
            this.Date = date.Date.ToString("yyyy/MM/dd");
            this.SCategory = scategory;
            this.DCategory = dcategory;
            this.Storagetype = storagetype;
            this.IsOutGoings = isoutogoings;
            this.CategoryData = string.Format("{0}>{1}>{2}",isoutogoings, scategory, dcategory);
        }


        public static HouseholdAccountsItem ToHouseholdaccountsItem(VMHouseholdAccountsItem item)
        {
            Regex re = new Regex(@"[^0-9]");
            var id = item.ID;
            var name = item.Name;
            var price = int.Parse(re.Replace(item.Price, ""));
            var date = DateTime.Parse(item.Date);
            var scategory = (SCategorys)Enum.Parse(typeof(SCategorys), item.SCategory);
            var dcategory = (DCategorys)Enum.Parse(typeof(DCategorys), item.DCategory);
            var storagetype = (StorageTypes)Enum.Parse(typeof(StorageTypes), item.Storagetype);
            var isoutgoings = string.Equals(item.IsOutGoings, "支出");
            return new HouseholdAccountsItem { ID = id, Name = name, Price = price, Date = date, DCategory = dcategory, SCategory = scategory, StorageType = storagetype, IsOutGoings = isoutgoings};
        }
    }
}
