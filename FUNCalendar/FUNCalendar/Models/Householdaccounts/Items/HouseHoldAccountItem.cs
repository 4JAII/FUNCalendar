using System;
using Prism.Mvvm;

namespace FUNCalendar.Models
{
    public class HouseHoldAccountsItem
    {
        private int _id;                                // ID
        private string _name;                           //商品名
        private int _price;                             //金額
        private DateTime _date;                         //日付
        private SCategorys _scategory;      //購入品の概要カテゴリー
        private DCategorys _dcategory;        //購入品の詳細カテゴリー
        private StorageTypes _storagetype;              //お金の所在地
        private bool _isOutGoings = true;               //収入か支出か

        public int ID
        {
            get { return this._id; }
            set { this._id = value; }
        }
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

        public HouseHoldAccountsItem(int id, string name, int price, DateTime date, DCategorys detailcategory, SCategorys summarycategory, StorageTypes storagetype, bool isoutgoings)
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
        public HouseHoldAccountsItem(string name, int price, DateTime date, DCategorys detailcategory, SCategorys summarycategory, StorageTypes storagetype, bool isoutgoings)
        {
            Name = name;
            Price = price;
            Date = date;
            DCategory = detailcategory;
            SCategory = summarycategory;
            StorageType = storagetype;
            IsOutGoings = isoutgoings;
        }

        /* Sort */
        public static int CompareByID(HouseHoldAccountsItem a, HouseHoldAccountsItem b)
        {
            return a.ID.CompareTo(b.ID);
        }
        public static int CompareByName(HouseHoldAccountsItem a, HouseHoldAccountsItem b)
        {
            return a.Name.CompareTo(b.Name);
        }
        public static int CompareByPrice(HouseHoldAccountsItem a, HouseHoldAccountsItem b)
        {
            return a.Price.CompareTo(b.Price);
        }
        public static int CompareByDate(HouseHoldAccountsItem a, HouseHoldAccountsItem b)
        {
            return a.Date.CompareTo(b.Date);
        }
    }
}