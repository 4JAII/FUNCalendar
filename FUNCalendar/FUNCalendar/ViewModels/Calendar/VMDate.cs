using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Mvvm;
using FUNCalendar.Models;
using System.Text.RegularExpressions;
using Xamarin.Forms;
using System.Collections.ObjectModel;

namespace FUNCalendar.ViewModels
{
    public class VMDate
    {
        private String[] dayOfTheWeekArray = { "日", "月", "火", "水", "木", "金", "土" };

        public DateTime DateData { get; private set; }
        public string StrYear { get; private set; }
        public string StrMonth { get; private set; }
        public string StrDate { get; private set; }
        public ImageSource HasWishList { get; private set; }
        public ImageSource HasToDoList { get; private set; }
        public ImageSource HasHouseHoldAccountsList { get; private set; }
        public string DayOfTheWeek { get; private set; }
        public Color BackgroundColor { get; private set; }

        /* Date=>VMDateに変換 */
        public VMDate(Date date)
        {
            this.DateData = date.DateData;
            this.StrYear = date.DateData.ToString("yyyy");
            this.StrMonth = date.DateData.ToString("%M");
            this.StrDate = date.DateData.ToString("%d");
            this.HasWishList = date.HasWishList ? ImageSource.FromFile("Calendar_HasWishList.png") : null;
            this.HasToDoList = date.HasToDoList ? ImageSource.FromFile("Calendar_HasToDoList.png") : null;
            this.HasHouseHoldAccountsList = date.HasHouseHoldAccountsList ? ImageSource.FromFile("Calendar_HasHouseHoldAccountsList.png") : null;
            this.DayOfTheWeek = dayOfTheWeekArray[(int)date.DayOfTheWeek];
            switch((int)date.DayOfTheWeek)
            {
                case 0 : this.BackgroundColor = Color.LightPink; break;
                case 6 : this.BackgroundColor = Color.LightBlue; break;
                default: this.BackgroundColor = Color.LightGray; break;
            }
        }

        /* 変換 */
        public static Date ToDate(VMDate vmDate)
        {
            return new Date(vmDate.DateData);
        }
    }
}
