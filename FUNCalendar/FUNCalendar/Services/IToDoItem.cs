using System;

namespace FUNCalendar.Models
{
    interface IToDoItem
    {
        int ID { get; set; }
        string Name { get; set; }
        DateTime Date { get; set; }
        int Priority { get; set; }
        bool IsCompleted { get; set; }
        int WishItemID { get; set; }

    }
}