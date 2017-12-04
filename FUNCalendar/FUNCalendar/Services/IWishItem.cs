using System;

namespace FUNCalendar.Models
{
    interface IWishItem
    {
        int ID { get; set; }
        string Name { get; set; }
        int Price { get; set; }
        DateTime Date { get; set; }
        bool IsBought { get; set; }

    }
}
