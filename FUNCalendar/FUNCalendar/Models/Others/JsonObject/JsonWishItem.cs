using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using FUNCalendar.Models;
using FUNCalendar.ViewModels;

namespace FUNCalendar.Models
{
    public class JsonWishItem
    {
        /* jsonのresultキーに対応 */
        private class Result
        {
            [JsonProperty("wish_item")]
            public VMWishItem VMValue { get; set; }
        }

        private Result result;
        [JsonProperty("result")]
        private Result ResultValue
        {
            set
            {
                result = value;
                this.VMValue = result.VMValue;
            }
        }
        [JsonProperty("wish_item")]
        public VMWishItem VMValue { get; set; }

        [JsonIgnore]
        public WishItem Value { get { return VMWishItem.ToWishItem(VMValue); } }
    }
}
