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
    public class JsonBalanceList
    {
        private class Result
        {
            [JsonProperty("balance_item")]
            public List<VMHouseholdAccountsBalanceItem> VMValue { get; set; }
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
        public List<VMHouseholdAccountsBalanceItem> VMValue { get; set; }
        [JsonIgnore]
        public List<HouseholdAccountsBalanceItem> Value { get { return VMValue.Select(x=> VMHouseholdAccountsBalanceItem.ToHouseholdAccountsBalanceItem(x)).ToList(); } }

    }
}
