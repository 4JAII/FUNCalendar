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
    public class JsonHouseholdAccountsList
    {
        private class Result
        {
            [JsonProperty("household_accounts_item")]
            public List<VMHouseholdAccountsItem> VMValue { get; set; }
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
        [JsonProperty("household_accounts_item")]
        public List<VMHouseholdAccountsItem> VMValue { get; set; }

        [JsonIgnore]
        public List<HouseholdAccountsItem> Value { get { return VMValue.Select(x => VMHouseholdAccountsItem.ToHouseholdaccountsItem(x)).ToList(); } }
    }
}
