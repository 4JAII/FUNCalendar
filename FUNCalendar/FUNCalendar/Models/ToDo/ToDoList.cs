using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Prism.Mvvm;
using Xamarin.Forms;
using System.Threading.Tasks;
using FUNCalendar.Services;

namespace FUNCalendar.Models
{
    public class ToDoList : BindableBase, IToDoList
    {
        private List<ToDoItem> allToDoList;
        private Func<ToDoItem, ToDoItem, int> selectedSortMethod;
        public ObservableCollection<ToDoItem> SortedToDoList { get; private set; }
        public ToDoItem DisplayToDoItem { get; set; }

        /* 昇順か？*/
        private bool isAscending = true;
        public bool IsAscending
        {
            get { return this.isAscending; }
            set { this.SetProperty(ref this.isAscending, value); }
        }

        private static bool isInitialized = false;

        public ToDoList()
        {
            SortedToDoList = new ObservableCollection<ToDoItem>();
            allToDoList = new List<ToDoItem>();
            selectedSortMethod = ToDoItem.CompareByID;
        }

        /* リスト更新 */
        private void UpdateSortedList()
        {
            SortedToDoList.Clear();
            foreach (var p in allToDoList)
            {
                SortedToDoList.Add(p);
            }
        }

        /* 各種ソート */
        private void Sort()
        {
            int sign = IsAscending ? 1 : -1;
            if (allToDoList.Count <= 0) return;
            allToDoList.Sort((x, y) => sign * selectedSortMethod(x, y));
            UpdateSortedList();
        }

        public void SortByID()
        {
            selectedSortMethod = ToDoItem.CompareByID;
            Sort();
        }

        public void SortByName()
        {
            selectedSortMethod = ToDoItem.CompareByName;
            Sort();
        }

        public void SortByDate()
        {
            selectedSortMethod = ToDoItem.CompareByDate;
            Sort();
        }

        public void SortByPriority()
        {
            selectedSortMethod = ToDoItem.CompareByPriority;
            Sort();
        }

        public void InitializeList(List<ToDoItem> list)
        {
            if (isInitialized) return;
            this.allToDoList = list;
            isInitialized = true;
        }

        /* アイテム追加 */
        public void AddToDoItem(ToDoItem todoItem)
        {
            allToDoList.Add(todoItem);
            Sort();
        }

        /* 登録画面用のアイテムセット */
        public void SetDisplayToDoItem(ToDoItem todoItem)
        {
            DisplayToDoItem = todoItem;
        }

        /* アイテム削除 */
        public void Remove(ToDoItem todoItem)
        {
            if (allToDoList.Count == 1) allToDoList.RemoveAt(0);
            else allToDoList.RemoveAll(x => x.ID == todoItem.ID);
            UpdateSortedList();
        }

        /* 特定のIDを持つアイテムを編集 */
        public void EditToDoItem(ToDoItem deleteToDoItem, ToDoItem addToDoItem)
        {
            allToDoList.RemoveAll(item => item.ID == deleteToDoItem.ID);
            AddToDoItem(addToDoItem);
            Sort();
        }
    }
}
