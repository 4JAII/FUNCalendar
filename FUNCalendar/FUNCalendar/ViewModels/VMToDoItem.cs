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
    public class VMToDoItem
    {
        public int ID { get; private set; }
        public string Name { get; private set; }
        public string Date { get; private set; }
        public string Priority { get; private set; }
        public string IsCompleted { get; private set; }

        /* ToDoItem=>VMToDoItemに変換 */
        public VMToDoItem(ToDoItem todoItem)
        {
            this.ID = todoItem.ID;
            this.Name = todoItem.Name;
            this.Date = todoItem.Date.ToString("yyyy/MM/dd");
            this.Priority = string.Format("{0}", todoItem.Priority); /* ?? */
            this.IsCompleted = todoItem.IsCompleted ? "完了" : "未完了";
        }

        /* 編集する用 */
        public VMToDoItem(int id, string name, DateTime date, string priority, string isCompleted)
        {
            this.ID = id;
            this.Name = name;
            this.Date = date.ToString("yyyy/MM/dd");
            this.Priority = priority;
            this.IsCompleted = isCompleted;

        }

        /* 変換 */
        public static ToDoItem ToToDoItem(VMToDoItem vmToDoItem)
        {
            Regex re = new Regex(@"[^0-9]");
            var id = vmToDoItem.ID;
            var name = vmToDoItem.Name;
            var date = DateTime.Parse(vmToDoItem.Date);
            var priority = int.Parse(re.Replace(vmToDoItem.Priority, ""));/* 属性により不正な値は除去されている前提 */
            var isCompleted = string.Equals(vmToDoItem, "完了");
            return new ToDoItem(id, name, date, priority, isCompleted);
        }
    }
}
