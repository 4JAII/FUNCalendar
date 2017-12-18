using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Mvvm;
using FUNCalendar.Models;
using System.Text.RegularExpressions;

namespace FUNCalendar.ViewModels
{
    public class VMWishItem
    {
        public int ID { get; private set; }
        public string Name { get; private set; }
        public string Price { get; private set; }
        public string Date { get; private set; }
        public string IsBought { get; private set; }

        /* WishItem=>VMWishItemに変換 */
        public VMWishItem(WishItem wishItem)
        {
            this.ID = wishItem.ID;
            this.Name = wishItem.Name;
            this.Price = string.Format("{0}円", wishItem.Price);
            this.Date = wishItem.Date.ToString("yyyy/MM/dd");
            this.IsBought = wishItem.IsBought ? "購入済み" : "未購入";
        }

        /* 編集する用 */
        public VMWishItem(int id,string name,string price,DateTime date,string isBought)
        {
            this.ID = id;
            this.Name = name;
            this.Price = price;
            this.Date = date.ToString("yyyy/MM/dd");
            this.IsBought = isBought;

        }
        /* 変換 */
        public static WishItem ToWishItem(VMWishItem vmWishItem)
        {
            Regex re = new Regex(@"[^0-9]");
            var id = vmWishItem.ID;
            var name = vmWishItem.Name;
            var price = int.Parse(re.Replace(vmWishItem.Price,""));/* 属性により不正な値は除去されている前提 */
            var date = DateTime.Parse(vmWishItem.Date);
            var isBought = string.Equals(vmWishItem , "購入済み");
            return new WishItem(id,name, price, date, isBought);
        }
    }
}

