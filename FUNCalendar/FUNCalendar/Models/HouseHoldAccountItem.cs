using System;
using Prism.Mvvm;

namespace FUNCalendar.Models
{
    public enum SCategorys    //購入品の概略カテゴリーの表
    {
        start_of_支出 = 0,
        食費,
        日用雑貨,
        交通費,
        娯楽費,
        医療費,
        通信費,
        水道_光熱費,
        その他_支出,
        end_of_支出,

        start_of_収入,
        給料,
        投資収入,
        その他_収入,
        end_of_収入
    }
    public enum DCategorys {     //購入品の詳細カテゴリーの表
        start_of_食費,
        朝食 = 1,
        end_of_食費,

        start_of_日用雑貨,
        end_of_日用雑貨,

        start_of_交通費,
        end_of_交通費,

        start_of_娯楽費,
        end_of_娯楽費,

        start_of_医療費,
        end_of_医療費,

        start_of_通信費,
        end_of_通信費,

        start_of_水道_光熱費,
        end_of_水道_光熱費,

        start_of_その他_支出,
        end_of_その他_支出,

        start_of_給料,
        end_of_給料,

        start_of_投資収入,
        end_of_投資収入,

        start_of_その他_収入,
        end_of_その他_収入
    }
    public enum StorageTypes        //お金の所在地の表
    {
        財布,
        貯金,
        銀行,
        クレジットカード,
        その他
    }
    public class HouseHoldAccountsItem : BindableBase
    {
        private int _id;                                // ID
        private string _name;                           //商品名
        private int _count;                             //個数
        private int _price;                             //金額
        private DateTime _date;                         //日付
        private SCategorys _summarycategory;      //購入品の概要カテゴリー
        private DCategorys _detailcategory;        //購入品の詳細カテゴリー
        private StorageTypes _storagetype;              //お金の所在地
        private bool _isOutGoings = true;               //収入か支出か

        public int ID
        {
            get { return this._id; }
            set { this.SetProperty(ref this._id, value); }
        }
        public string Name
        {
            get { return this._name;}
            set { this.SetProperty(ref this._name, value); }
        }
        public int Count
        {
            get { return this._count; }
            set { this.SetProperty(ref this._count, value); }
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
        public SCategorys SummaryCategory
        {
            get { return this._summarycategory; }
            set { this.SetProperty(ref this._summarycategory, value); }
        }
        public DCategorys DetailCategory
        {
            get { return this._detailcategory; }
            set { this.SetProperty(ref this._detailcategory, value); }
        }
        public StorageTypes StorageType
        {
            get { return this._storagetype; }
            set { this.SetProperty(ref this._storagetype, value); }
        }

        public bool IsOutGoings
        {
            get { return this._isOutGoings;}
            set { this.SetProperty(ref this._isOutGoings, value); }
        }

        public HouseHoldAccountsItem(int id,string name,int price,DateTime date,DCategorys detailcategory,SCategorys summarycategory,StorageTypes storagetype,bool isoutgoings)
        {
            ID = id;
            Name = name;
            Price = price;
            Date = date;
            DetailCategory = detailcategory;
            SummaryCategory = summarycategory;
            StorageType = storagetype;
            IsOutGoings = isoutgoings;
        }
        public HouseHoldAccountsItem(string name, int price, DateTime date, DCategorys detailcategory, SCategorys summarycategory, StorageTypes storagetype, bool isoutgoings)
        {
            Name = name;
            Price = price;
            Date = date;
            DetailCategory = detailcategory;
            SummaryCategory = summarycategory;
            StorageType = storagetype;
            IsOutGoings = isoutgoings;
        }

        /* Sort */
        public static int CompareByID(HouseHoldAccountsItem a,HouseHoldAccountsItem b)
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