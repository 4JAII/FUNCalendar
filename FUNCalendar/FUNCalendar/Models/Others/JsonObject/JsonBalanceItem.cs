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
    public class JsonBalanceItem
    {
        private class Result
        {
            [JsonProperty("balance_item")]
            public VMHouseholdAccountsBalanceItem VMValue { get; set; }
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
        [JsonProperty("balance_item")]
        public VMHouseholdAccountsBalanceItem VMValue { get; set; }
        [JsonIgnore]
        public HouseholdAccountsBalanceItem Value { get { return VMHouseholdAccountsBalanceItem.ToHouseholdAccountsBalanceItem(VMValue); } }
    }
}
