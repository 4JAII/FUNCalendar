using System;
using Prism.Mvvm;
using SQLite;

namespace FUNCalendar.Models
{
    [Table("WishItem")]
    public class WishItem : BindableBase
    {
        /*プロパティ用変数定義*/
        private int _id;
        private string _name;
        private int _price;
        private DateTime _date;
        private bool _isBought;
        private int _todoId;

        /*プロパティ定義(変更通知機能あり)*/
        [PrimaryKey, AutoIncrement]
        public int ID
        {
            get { return this._id; }
            set { this.SetProperty(ref this._id, value); }
        }
        [MaxLength(32)]
        public string Name
        {
            get { return this._name; }
            set { this.SetProperty(ref this._name, value); }
        }

        public int Price
        {
            get { return this._price; }
            set { this.SetProperty(ref this._price, value); }
        }

        public DateTime Date
        {
            get { return this._date; }
            set { this.SetProperty(ref this._date, value); }
        }

        public bool IsBought
        {
            get { return this._isBought; }
            set { this.SetProperty(ref this._isBought, value); }
        }

        public int ToDoID
        {
            get { return this._todoId; }
            set { this.SetProperty(ref this._todoId, value); }
        }

        /*コンストラクタ*/
        public WishItem() { }

        public WishItem(int id, string name, int price, DateTime date, bool isBought, int todoId)
        {
            ID = id;
            Name = name;
            Price = price;
            Date = date;
            IsBought = isBought;
            ToDoID = todoId;
        }

        public WishItem(string name, int price, DateTime date, bool isBought, int todoId)
        {
            ID = 0;
            Name = name;
            Price = price;
            Date = date;
            IsBought = isBought;
            ToDoID = todoId;
        }

        /*静的メソッド定義*/
        public static int CompareByID(WishItem a, WishItem b)
        {
            return a.ID.CompareTo(b.ID);
        }

        public static int CompareByName(WishItem a, WishItem b)
        {
            return a.Name.CompareTo(b.Name);
        }

        public static int CompareByPrice(WishItem a, WishItem b)
        {
            return a.Price.CompareTo(b.Price);
        }

        public static int CompareByDate(WishItem a, WishItem b)
        {
            return a.Date.CompareTo(b.Date);
        }
    }
}
