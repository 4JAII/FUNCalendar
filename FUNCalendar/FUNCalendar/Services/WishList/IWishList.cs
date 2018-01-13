using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FUNCalendar.Models;

namespace FUNCalendar.Models
{
    public interface IWishList
    {
        ObservableCollection<WishItem> SortedWishList { get; }
        WishItem DisplayWishItem { get; set; }
        ObservableCollection<WishItem> WishListForCalendar { get; }
        bool DateWithWishList { get; }
        bool IsAscending { get; set; }
        void SortByID();
        void SortByName();
        void SortByPrice();
        void SortByDate();
        void AddWishItem(WishItem wishItem);
        void InitializeList(List<WishItem> list);
        void SetDisplayWishItem(WishItem wishItem);
        void Remove(WishItem wishItem);
        void EditWishItem(WishItem deleteWishItem,WishItem addWishItem);
        void SetWishListForCalendar(DateTime date);
        void ClearWishListForCalendar();
        void SetDateWithWishList(DateTime date);
    }
}
