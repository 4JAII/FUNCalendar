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
    public class JsonToDoList
    {
        private class Result
        {
            [JsonProperty("todo_item")]
            public List<VMToDoItem> VMValue { get; set; }
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
        public List<VMToDoItem> VMValue { get; set; }
        [JsonIgnore]
        public List<ToDoItem> Value { get { return VMValue.Select(x => VMToDoItem.ToToDoItem(x)).ToList(); } }
    }
}
