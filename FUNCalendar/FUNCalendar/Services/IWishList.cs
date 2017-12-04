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
        bool IsAscending { get; set; }
        int IDCount { get; }
        void SortByID();
        void SortByName();
        void SortByPrice();
        void SortByDate();
        void AddWishItem(string name, int price, DateTime date, bool isBought, bool isAddToDo);
        void AddWishItem(WishItem wishItem);
        void SetDisplayWishItem(WishItem wishItem);
        void Remove(WishItem wishItem);
        void EditWishItem(WishItem deleteWishItem,WishItem addWishItem);
    }
}
