using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Mvvm;
using FUNCalendar.Models;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace FUNCalendar.ViewModels
{
    [JsonObject("wish_item")]
    public class VMWishItem
    {
        [JsonProperty("id")]
        public int ID { get; private set; }
        [JsonProperty("name")]
        public string Name { get; private set; }
        [JsonProperty("price")]
        public string Price { get; private set; }
        [JsonProperty("date")]
        public string Date { get; private set; }
        [JsonProperty("is_bought")]
        public string IsBought { get; private set; }
        [JsonProperty("todo_id")]
        public int ToDoID { get; private set; }

        public VMWishItem() { }

        /* WishItem=>VMWishItemに変換 */
        public VMWishItem(WishItem wishItem)
        {
            this.ID = wishItem.ID;
            this.Name = wishItem.Name;
            this.Price = string.Format("{0}円", wishItem.Price);
            this.Date = wishItem.Date.ToString("yyyy/MM/dd");
            this.IsBought = wishItem.IsBought ? "購入済み" : "未購入";
            this.ToDoID = wishItem.ToDoID;
        }

        /* 編集する用 */
        public VMWishItem(int id, string name, string price, DateTime date, string isBought, int todoID)
        {
            this.ID = id;
            this.Name = name;
            this.Price = price;
            this.Date = date.ToString("yyyy/MM/dd");
            this.IsBought = isBought;
            this.ToDoID = todoID;

        }
        /* 変換 */
        public static WishItem ToWishItem(VMWishItem vmWishItem)
        {
            if (vmWishItem == null) return null;
            Regex re = new Regex(@"[^0-9]");
            var id = vmWishItem.ID;
            var name = vmWishItem.Name;
            var price = int.Parse(re.Replace(vmWishItem.Price, ""));/* 属性により不正な値は除去されている前提 */
            var date = DateTime.Parse(vmWishItem.Date);
            var isBought = string.Equals(vmWishItem.IsBought, "購入済み");
            var todoID = vmWishItem.ToDoID;
            return new WishItem { ID = id, Name = name, Price = price, Date = date, IsBought = isBought, ToDoID = todoID };
        }
    }
}

