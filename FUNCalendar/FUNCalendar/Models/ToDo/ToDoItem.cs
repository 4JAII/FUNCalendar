using System;
using Prism.Mvvm;
using SQLite;

namespace FUNCalendar.Models
{
    [Table("ToDoItem")]
    public class ToDoItem : BindableBase
    {
        /*プロパティ用変数定義*/
        private int _id;
        private string _description;
        private DateTime _date;
        private int _priority;
        private bool _isCompleted;
        private int _wishID;

        /*プロパティ定義(変更通知機能あり)*/
        [PrimaryKey, AutoIncrement]
        public int ID
        {
            get { return this._id; }
            set { this.SetProperty(ref this._id, value); }
        }
        [MaxLength(32)]
        public string Description
        {
            get { return this._description; }
            set { this.SetProperty(ref this._description, value); }
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

        public int WishID
        {
            get { return this._wishID; }
            set { this.SetProperty(ref this._wishID, value); }
        }

        /*コンストラクタ*/
        public ToDoItem() { }

        public ToDoItem(int id, string description, DateTime date, int priority, bool isCompleted, int wishId)
        {
            ID = id;
            Description = description;
            Date = date;
            Priority = priority;
            IsCompleted = isCompleted;
            WishID = wishId;
        }

        public ToDoItem(string description, DateTime date, int priority, bool isCompleted, int wishId)
        {
            ID = 0;
            Description = description;
            Date = date;
            Priority = priority;
            IsCompleted = isCompleted;
            WishID = wishId;
        }

        /*静的メソッド定義*/
        public static int CompareByID(ToDoItem a, ToDoItem b)
        {
            return a.ID.CompareTo(b.ID);
        }

        public static int CompareByName(ToDoItem a, ToDoItem b)
        {
            return a.Description.CompareTo(b.Description);
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
