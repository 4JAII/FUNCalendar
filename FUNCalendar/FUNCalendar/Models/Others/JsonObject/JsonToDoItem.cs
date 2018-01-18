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
    public class JsonToDoItem
    {
        private class Result
        {
            [JsonProperty("todo_item")]
            public VMToDoItem VMValue { get; set; }
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
        [JsonProperty("todo_item")]
        public VMToDoItem VMValue { get; set; }

        [JsonIgnore]
        public ToDoItem Value { get { return VMToDoItem.ToToDoItem(VMValue); } }
    }
}
