using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Prism.Mvvm;
using FUNCalendar.Services;

namespace FUNCalendar.Models
{
    public class Calendar : BindableBase, ICalendar
    {
        private static readonly int dateMaxValue = 42; /* 縦6横7のカレンダー */
        private Date[] aMonthDateData = new Date[dateMaxValue];
        private int currentMonth, currentYear;
        private int[] monthDateMaxValue = { 31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31 };
        private int i, j;
        private List<Date> tempList = new List<Date>();
        private IWishList wishList;
        private IToDoList todoList;
        private IHouseholdAccounts householdAccounts;

        public ExtendObservableCollection<Date> ListedAMonthDateData { get; private set; } = new ExtendObservableCollection<Date>();
        public Date DisplayDate { get; set; }

        /* プロパティ */
        public Date[] AMonthDateData
        {
            get { return this.aMonthDateData; }
            set { this.SetProperty(ref this.aMonthDateData, value); }
        }

        public int CurrentMonth
        {
            get { return this.currentMonth; }
            set { this.SetProperty(ref this.currentMonth, value); }
        }

        public int CurrentYear
        {
            get { return this.currentYear; }
            set { this.SetProperty(ref this.currentYear, value); }
        }

        /* コンストラクタ */
        public Calendar()
        {
            currentMonth = DateTime.Now.Month;
            currentYear = DateTime.Now.Year;
            for (int i = 0; i < dateMaxValue; i++)
            {
                aMonthDateData[i] = new Date();
            }
        }

        /* メソッド */

        /* 一か月分のデータをaMonthDateDataに格納 */
        public void Update()
        {
            /* その月の1日が何曜日か */
            DayOfWeek firstDayOfTheWeek = new DateTime(currentYear, currentMonth, 1).DayOfWeek;

            /* うるう年の判定 */
            if (DateTime.IsLeapYear(currentYear))
            {
                monthDateMaxValue[1] = 29;
            }
            else
            {
                monthDateMaxValue[1] = 28;
            }

            /* Dateプロパティにアクセス */
            /* 前の月の分を格納 */
            if (currentMonth - 1 == 0)
            {
                j = 1;
            }
            else
            {
                j = 0;
            }

            for (i = 0; i < (int)firstDayOfTheWeek; i++)
            {
                aMonthDateData[i].DateData = new DateTime(currentYear - j, currentMonth - 1 + j * 12, monthDateMaxValue[currentMonth + j * 12 - 2] - (int)firstDayOfTheWeek + i + 1);
            }
            /* 現在の月の分を格納 */
            for (i = 0; i < monthDateMaxValue[currentMonth - 1]; i++)
            {
                aMonthDateData[i + (int)firstDayOfTheWeek].DateData = new DateTime(currentYear, currentMonth, i + 1);
            }
            /* 次の月の分を格納 */
            if (currentMonth + 1 == 13)
            {
                j = 1;
            }
            else
            {
                j = 0;
            }
            for (i = (int)firstDayOfTheWeek + monthDateMaxValue[currentMonth - 1]; i < dateMaxValue; i++)
            {
                aMonthDateData[i].DateData = new DateTime(currentYear + j, currentMonth + 1 - j * 12, i - (int)firstDayOfTheWeek - monthDateMaxValue[currentMonth - 1] + 1);
            }

            SetHasList();
            ListedAMonthDateData.Replace(aMonthDateData);
        }

        /* 一つ前の月へ */
        public void BackPrevMonth()
        {
            currentMonth--;
            if (currentMonth < 1)
            {
                currentMonth = 12;
                currentYear--;
            }
            Update();
        }

        /* 一つ次の月へ */
        public void GoNextMonth()
        {
            currentMonth++;
            if (currentMonth > 12)
            {
                currentMonth = 1;
                currentYear++;
            }
            Update();
        }

        /* 詳細画面用のアイテムセット */
        public void SetDisplayDate(Date date)
        {
            DisplayDate = date;
        }

        /* WishList,ToDoList,HouseholdAccountsListを持っているか登録 */
        private void SetHasList()
        {
            foreach (Date x in AMonthDateData)
            {
                wishList.SetDateWithWishList(x.DateData);
                x.HasWishList = wishList.DateWithWishList;
                todoList.SetDateWithToDoList(x.DateData);
                x.HasToDoList = todoList.DateWithToDoList;
                householdAccounts.SetDateWithHouseholdAccounts(x.DateData);
                x.HasHouseHoldAccountsList = householdAccounts.DateWithHouseholdAccounts;
            }
        }

        /* WishList,ToDoList,HouseholdAccountsListをもらう */
        public void SetLists(IWishList wishList, IToDoList todoList, IHouseholdAccounts householdAccounts)
        {
            this.wishList = wishList;
            this.todoList = todoList;
            this.householdAccounts = householdAccounts;
            Update();
        }
    }
}
