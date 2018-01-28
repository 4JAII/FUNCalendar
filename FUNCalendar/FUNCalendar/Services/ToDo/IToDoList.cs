using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FUNCalendar.Models;

namespace FUNCalendar.Services
{
    public interface IToDoList:INotifyPropertyChanged
    {
        ExtendedObservableCollection<ToDoItem> SortedToDoList { get; }
        ToDoItem DisplayToDoItem { get; set; }
        bool IsAscending { get;  }
        void SortByID();
        void SortByName();
        void SortByDate();
        void SortByPriority();
        void Reverse();
        void AddToDoItem(ToDoItem todoItem);
        void UpdateList(List<ToDoItem> list);
        void SetDisplayToDoItem(ToDoItem todoItem);
        void Remove(ToDoItem todoItem);
        void EditToDoItem(ToDoItem deleteToDoItem, ToDoItem addToDoItem);
    }
}