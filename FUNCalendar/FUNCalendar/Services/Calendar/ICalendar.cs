using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FUNCalendar.Models;

namespace FUNCalendar.Services
{
    public interface ICalendar
    {
        Date[] AMonthDateData { get; set; }
        ExtendObservableCollection<Date> ListedAMonthDateData { get; }
        Date DisplayDate { get; set; }
        int CurrentMonth { get; set; }
        int CurrentYear { get; set; }
        void Update();
        void BackPrevMonth();
        void GoNextMonth();
        void SetDisplayDate(Date date);
        void SetHasList(IWishList wishList);
    }
}
