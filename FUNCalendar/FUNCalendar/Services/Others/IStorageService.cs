using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FUNCalendar.Models;

namespace FUNCalendar.Services
{
    interface IStorageService
    {
        int LastAddedWishItemID { get; }
        int LastAddedToDoItemID { get; }
        int LastAddedHouseHoldAccountsID { get; }

        Task<bool> AddItem(WishItem item);
        //Task<bool> AddItem(ToDoItem item);
        //Task<bool> AddItem(BalanceItem item);
        //Task<bool> AddItem(HouseHoldAccountsItem item);
        Task<bool> DeleteItem(WishItem item);
        //Task<bool> DeleteItem(ToDoItem item);
        //Task<bool> DeleteItem(HouseHoldAccountsItem item);
        Task<bool> EditItem(WishItem item);
        //Task<bool> EditItem(ToDoItem item);
        //Task<bool> EditItem(HouseHoldAccountsItem item);
        Task<List<WishItem>> ReadFile();
        //Task<List<ToDoItem>> ReadFile();
        //Task<List<BalanceItem>> ReadFile();
        //Task<List<HouseHoldAccountsItem>> ReadFile();

    }
}
