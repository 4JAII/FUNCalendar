using System;
using System.Collections.Generic;
using Prism.Mvvm;

namespace FUNCalendar.Models
{
    public class Date : BindableBase
{
        /* privateで定義 */
        private DateTime dateData;
        private List<WishItem> wishList = new List<WishItem>();
        private List</*ToDoItem*/string> todoList = new List</*ToDoItem*/string>();
        private List</*家計簿*/string> houseHoldAccountsList = new List</*家計簿*/string>();
        private DayOfWeek dayOfTheWeek;

        /* プロパティ */
        public DateTime DateData
        {
            get
            {
                return this.dateData;
            }
            set
            {
                this.SetProperty(ref this.dateData, value);
                DayOfTheWeek = this.dateData.DayOfWeek;
            }
        }

        public List<WishItem> WishList
        {
            get { return this.wishList; }
            set { this.SetProperty(ref this.wishList, value); }
        }

        public List</*ToDoItem*/string> ToDoList
        {
            get { return this.todoList; }
            set { this.SetProperty(ref this.todoList,value); }
        }

        public List</*家計簿*/string> HouseHoldAccountsList
        {
            get { return this.houseHoldAccountsList; }
            set { this.SetProperty(ref this.houseHoldAccountsList,value); }
        }

        public DayOfWeek DayOfTheWeek
        {
            get { return this.dayOfTheWeek; }
            private set { this.dayOfTheWeek = value; }
        }

        /* コンストラクタ */
        public Date(DateTime dateData)
        {
            this.dateData = dateData;
        }

        public Date()
        {
        }
    }
}
