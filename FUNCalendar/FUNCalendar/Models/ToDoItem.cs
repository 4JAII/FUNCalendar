using System;
using Prism.Mvvm;

namespace FUNCalendar.Models
{
    public class ToDoItem : BindableBase,IToDoItem
    {
        /*プロパティ用変数定義*/
        private int _id;
        private string _name;
        private DateTime _date;
        private int _priority;
        private bool _isCompleted;
        private int _wishItemID;

        /*プロパティ定義(変更通知機能あり)*/
        public int ID
        {
            get { return this._id; }
            set { this.SetProperty(ref this._id, value); }
        }

        public string Name
        {
            get { return this._name; }
            set { this.SetProperty(ref this._name, value); }
        }

        public DateTime Date
        {
            get { return this._date; }
            set { this.SetProperty(ref this._date, value); }
        }

        public int Priority
        {
            get { return this._priority; }
            set { this.SetProperty(ref this._priority, value); }
        }

        public bool IsCompleted
        {
            get { return this._isCompleted; }
            set { this.SetProperty(ref this._isCompleted, value); }
        }

        public int WishItemID
        {
            get { return this._wishItemID; }
            set { this.SetProperty(ref this._wishItemID, value); }
        }

        /*コンストラクタ*/
        public ToDoItem() { }

        public ToDoItem(int id, string name, DateTime date, int priority, bool isCompleted)
        {
            ID = id;
            Name = name;
            Date = date;
            Priority = priority;
            IsCompleted = isCompleted;
        }

        public ToDoItem(string name, DateTime date, int priority, bool isCompleted)
        {
            Name = name;
            Date = date;
            Priority = priority;
            IsCompleted = isCompleted;
        }

        public ToDoItem(int id, string name, DateTime date, int priority, bool isCompleted, int wishItemId)
        {
            ID = id;
            Name = name;
            Date = date;
            Priority = priority;
            IsCompleted = isCompleted;
            WishItemID = wishItemId;
        }

        public ToDoItem(string name, DateTime date, int priority, bool isCompleted, int wishItemId)
        {
            Name = name;
            Date = date;
            Priority = priority;
            IsCompleted = isCompleted;
            WishItemID = wishItemId;
        }

        /*静的メソッド定義*/
        public static int CompareByID(ToDoItem a, ToDoItem b)
        {
            return a.ID.CompareTo(b.ID);
        }

        public static int CompareByName(ToDoItem a, ToDoItem b)
        {
            return a.Name.CompareTo(b.Name);
        }

        public static int CompareByDate(ToDoItem a, ToDoItem b)
        {
            return a.Date.CompareTo(b.Date);
        }

        public static int CompareByPriority(ToDoItem a, ToDoItem b)
        {
            return a.Priority.CompareTo(b.Priority);
        }
    }
}
