using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FUNCalendar.Models;

namespace FUNCalendar.ViewModels
{
    public class ToDoListSortName
    {
        public string SortName { get; set; }
        public Action Sort { get; set; }
    }
}