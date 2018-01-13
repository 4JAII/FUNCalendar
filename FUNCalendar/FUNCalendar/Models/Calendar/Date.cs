using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Prism.Mvvm;

namespace FUNCalendar.Models
{
    public class Date : BindableBase
{
        /* privateで定義 */
        private DateTime dateData;
        private bool hasWishList;
        private bool hasToDoList;
        private bool hasHouseHoldAccountsList;
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

        public bool HasWishList
        {
            get { return this.hasWishList; }
            set { this.SetProperty(ref this.hasWishList, value); }
        }

        public bool HasToDoList
        {
            get { return this.hasToDoList; }
            set { this.SetProperty(ref this.hasToDoList, value); }
        }

        public bool HasHouseHoldAccountsList
        {
            get { return this.hasHouseHoldAccountsList; }
            set { this.SetProperty(ref this.hasHouseHoldAccountsList, value); }
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
