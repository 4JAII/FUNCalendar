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
    public class JsonHouseholdAccountsItem
    {
        private class Result
        {
            [JsonProperty("household_accounts_item")]
            public VMHouseholdAccountsItem VMValue { get; set; }
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
        public VMHouseholdAccountsItem VMValue { get; set; }

        [JsonIgnore]
        public HouseholdAccountsItem Value { get { return VMHouseholdAccountsItem.ToHouseholdaccountsItem(VMValue); } }
    }
}
