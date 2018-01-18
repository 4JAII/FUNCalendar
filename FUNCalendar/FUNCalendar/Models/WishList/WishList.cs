using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Prism.Mvvm;
using Xamarin.Forms;
using System.Threading.Tasks;
using FUNCalendar.Services;

namespace FUNCalendar.Models
{

    public class WishList : BindableBase, IWishList
    {
        private List<WishItem> allWishList;
        private Func<WishItem, WishItem, int> selectedSortMethod;
        public ObservableCollection<WishItem> SortedWishList { get; private set; }
        public WishItem DisplayWishItem { get; set; }

        /* 昇順か？*/
        private bool isAscending = true;
        public bool IsAscending
        {
            get { return this.isAscending; }
            set { this.SetProperty(ref this.isAscending, value); }
        }


        public WishList()
        {
            SortedWishList = new ObservableCollection<WishItem>();
            allWishList = new List<WishItem>();
            selectedSortMethod = WishItem.CompareByID;
        }


        /* リスト更新 */
        private void UpdateSortedList()
        {
            SortedWishList.Clear();
            foreach (var p in allWishList)
            {
                SortedWishList.Add(p);
            }
        }

        /* 各種ソート */
        private void Sort()
        {
            int sign = IsAscending ? 1 : -1;
            if (allWishList == null) return;
            if (allWishList.Count <= 0) return;
            allWishList.Sort((x, y) => sign * selectedSortMethod(x, y));
            UpdateSortedList();
        }

        public void SortByID()
        {
            selectedSortMethod = WishItem.CompareByID;
            Sort();
        }

        public void SortByName()
        {
            selectedSortMethod = WishItem.CompareByName;
            Sort();
        }

        public void SortByPrice()
        {
            selectedSortMethod = WishItem.CompareByPrice;
            Sort();
        }

        public void SortByDate()
        {
            selectedSortMethod = WishItem.CompareByDate;
            Sort();
        }
        public void UpdateList(List<WishItem> list)
        {
            this.allWishList = list;
        }

        /* アイテム追加 */
        public void AddWishItem(WishItem wishItem)
        {
            allWishList.Add(wishItem);
            Sort();
        }

        /* 登録画面用のアイテムセット */
        public void SetDisplayWishItem(WishItem wishItem)
        {
            DisplayWishItem = wishItem;
        }

        /* アイテム削除 */
        public void Remove(WishItem wishItem)
        {
            if (allWishList.Count == 1) allWishList.RemoveAt(0);
            else allWishList.RemoveAll(x => x.ID == wishItem.ID);
            UpdateSortedList();
        }

        /* 特定のIDを持つアイテムを編集 */
        public void EditWishItem(WishItem deleteWishItem, WishItem addWishItem)
        {
            allWishList.RemoveAll(item => item.ID == deleteWishItem.ID);
            AddWishItem(addWishItem);
            Sort();
        }
    }
}
