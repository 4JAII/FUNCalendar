using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FUNCalendar.Models;

namespace FUNCalendar.Services
{
    public interface IWishList
    {
        ObservableCollection<WishItem> SortedWishList { get; }
        WishItem DisplayWishItem { get; set; }
        bool IsAscending { get; set; }
        void SortByID();
        void SortByName();
        void SortByPrice();
        void SortByDate();
        void AddWishItem(WishItem wishItem);
        void UpdateList(List<WishItem> list);
        void SetDisplayWishItem(WishItem wishItem);
        void SetDisplayWishItem(int wishID);
        void Remove(WishItem wishItem);
        void EditWishItem(WishItem deleteWishItem,WishItem addWishItem);
    }
}
