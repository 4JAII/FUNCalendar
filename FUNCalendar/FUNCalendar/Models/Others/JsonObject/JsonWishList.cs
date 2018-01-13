using FUNCalendar.ViewModels;
using FUNCalendar.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace FUNCalendar.Models
{
    public class JsonWishList
    {
        /* jsonのresultキーに対応 */
        private class Result
        {
            [JsonProperty("wish_item")]
            public List<VMWishItem> VMValue { get; set; }    
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
        public List<VMWishItem> VMValue { get; set; }

        [JsonIgnore]
        public List<WishItem> Value
        {
            get { return VMValue.Select(x => VMWishItem.ToWishItem(x)).ToList(); }
        }
    }
}
