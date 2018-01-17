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
    [JsonObject("todo_item")]
    public class VMToDoItem
    {
        [JsonProperty("id")]
        public int ID { get; private set; }
        [JsonProperty("description")]
        public string Description { get; private set; }
        [JsonProperty("date")]
        public string Date { get; private set; }
        [JsonProperty("priority")]
        public string Priority { get; private set; }
        [JsonProperty("is_completed")]
        public string IsCompleted { get; private set; }
        [JsonProperty("wish_id")]
        public int WishID { get; private set; }

        public VMToDoItem() { }

        /* ToDoItem=>VMToDoItemに変換 */
        public VMToDoItem(ToDoItem todoItem)
        {
            this.ID = todoItem.ID;
            this.Description = todoItem.Description;
            this.Date = todoItem.Date.ToString("yyyy/MM/dd");
            this.Priority = string.Format("{0}", todoItem.Priority);
            this.IsCompleted = todoItem.IsCompleted ? "完了" : "未完了";
            this.WishID = todoItem.WishID;
        }

        /* 編集する用 */
        public VMToDoItem(int id, string description, DateTime date, string priority, string isCompleted,int wishID)
        {
            this.ID = id;
            this.Description = description;
            this.Date = date.ToString("yyyy/MM/dd");
            this.Priority = priority;
            this.IsCompleted = isCompleted;
            this.WishID = WishID;
        }

        /* 変換 */
        public static ToDoItem ToToDoItem(VMToDoItem vmToDoItem)
        {
            if (vmToDoItem == null) return null;
            Regex re = new Regex(@"[^0-9]");
            var id = vmToDoItem.ID;
            var description = vmToDoItem.Description;
            var date = DateTime.Parse(vmToDoItem.Date);
            var priority = int.Parse(re.Replace(vmToDoItem.Priority, ""));/* 属性により不正な値は除去されている前提 */
            var isCompleted = string.Equals(vmToDoItem, "完了");
            var wishID = vmToDoItem.WishID;
            return new ToDoItem { ID = id, Description = description, Date = date, Priority = priority, IsCompleted = isCompleted, WishID = wishID };
        }
    }
}
