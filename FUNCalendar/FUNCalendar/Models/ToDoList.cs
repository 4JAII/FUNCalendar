using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Prism.Mvvm;
using Xamarin.Forms;
using FUNCalendar.Models;

namespace FUNCalendar.Models
{
    public class ToDoList : BindableBase,IToDoList
    {
        private List<ToDoItem> allToDoList;
        private Func<ToDoItem, ToDoItem, int> selectedSortMethod;
        /* private データベース操作 変数 */

        public ObservableCollection<ToDoItem> SortedToDoList { get; private set; }
        public ToDoItem DisplayToDoItem { get; set; }

        /* 昇順か？*/
        private bool isAscending = true;
        public bool IsAscending
        {
            get { return this.isAscending; }
            set { this.SetProperty(ref this.isAscending, value); }
        }

        /* リストが渡されたときのコンストラクタ(ViewModelにDI？(インタフェースなし)されたリストを引数とする) */
        /*public ToDoList(List<ToDoItem> list)
        {
            allToDoList = list;
            SortedToDoList = new ObservableCollection<ToDoItem>();
            SortByID();
        }*/

        /* ToDoItemが何個か渡されたときのコンストラクタ(デバッグ用) */
        /*public ToDoList(params ToDoItem[] todoItems)
        {
            allToDoList = new List<ToDoItem>();
            SortedToDoList = new ObservableCollection<ToDoItem>();
            foreach (var p in todoItems)
            {
                allToDoList.Add(p);
            }
            SortByID();
        }*/

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

        /* アイテム追加 */
        public void AddToDoItem(ToDoItem todoItem)
        {
            allToDoList.Add(todoItem);
            /* データベースにIDCount allToDoList 書き出し　コスト高めなら要検討 */
            Sort();
        }

        /* 詳細画面用のアイテムセット */
        public void SetDisplayToDoItem(ToDoItem todoItem)
        {
            DisplayToDoItem = todoItem;
        }

        /* 特定のIDをもつアイテム削除 */
        public void Remove(ToDoItem todoItem)
        {
            allToDoList.Remove(todoItem);
            Sort();
        }

        /* 特定のIDを持つアイテムを編集 */
        public void EditToDoItem(ToDoItem deleteToDoItem, ToDoItem addToDoItem)
        {
            allToDoList.RemoveAll((item) => item.ID == deleteToDoItem.ID);
            AddToDoItem(addToDoItem);
            Sort();
        }
    }
}
