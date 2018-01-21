using System;
using Prism.Mvvm;
using FUNCalendar.ViewModels;
using SQLite;


namespace FUNCalendar.Models
{
    [Table("HouseholdAccountsItem")]
    public class HouseholdAccountsItem
    {
        private int _id;                                // ID
        private string _name;                           //商品名
        private int _price;                             //金額
        private DateTime _date;                         //日付
        private SCategorys _scategory;      //購入品の概要カテゴリー
        private DCategorys _dcategory;        //購入品の詳細カテゴリー
        private StorageTypes _storagetype;              //お金の所在地
        private bool _isOutGoings = true;               //収入か支出か

        [PrimaryKey, AutoIncrement,]
        public int ID
        {
            get { return this._id; }
            set { this._id = value; }
        }
        [MaxLength(32)]
        public string Name
        {
            get { return this._name; }
            set { this._name = value; }
        }
        public int Price
        {
            get { return this._price; }
            set { this._price = value; }
        }
        public DateTime Date
        {
            get { return this._date; }
            set { this._date = value; }
        }
        public SCategorys SCategory
        {
            get { return this._scategory; }
            set { this._scategory = value; }
        }
        public DCategorys DCategory
        {
            get { return this._dcategory; }
            set { this._dcategory = value; }
        }
        public StorageTypes StorageType
        {
            get { return this._storagetype; }
            set { this._storagetype = value; }
        }

        public bool IsOutGoings
        {
            get { return this._isOutGoings; }
            set { this._isOutGoings = value; }
        }

        public HouseholdAccountsItem() { }

        public HouseholdAccountsItem(int id, string name, int price, DateTime date, DCategorys detailcategory, SCategorys summarycategory, StorageTypes storagetype, bool isoutgoings)
        {
            ID = id;
            Name = name;
            Price = price;
            Date = date;
            DCategory = detailcategory;
            SCategory = summarycategory;
            StorageType = storagetype;
            IsOutGoings = isoutgoings;
        }
        public HouseholdAccountsItem(string name, int price, DateTime date, DCategorys detailcategory, SCategorys summarycategory, StorageTypes storagetype, bool isoutgoings)
        {
            //ID = 0;
            Name = name;
            Price = price;
            Date = date;
            DCategory = detailcategory;
            SCategory = summarycategory;
            StorageType = storagetype;
            IsOutGoings = isoutgoings;
        }

       

        /* Sort */
        public static int CompareByID(HouseholdAccountsItem a, HouseholdAccountsItem b)
        {
            return a.ID.CompareTo(b.ID);
        }
        public static int CompareByName(HouseholdAccountsItem a, HouseholdAccountsItem b)
        {
            return a.Name.CompareTo(b.Name);
        }
        public static int CompareByPrice(HouseholdAccountsItem a, HouseholdAccountsItem b)
        {
            return a.Price.CompareTo(b.Price);
        }
        public static int CompareByDate(HouseholdAccountsItem a, HouseholdAccountsItem b)
        {
            return a.Date.CompareTo(b.Date);
        }
    }
}