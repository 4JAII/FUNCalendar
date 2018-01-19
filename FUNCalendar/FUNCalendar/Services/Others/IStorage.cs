using FUNCalendar.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FUNCalendar.Services
{
    public interface IStorage
    {
        int LastAddedWishItemID { get; }
        int LastAddedToDoItemID { get; }
        int LastAddedHouseholdAccountsItemID { get; }

        Task<bool> AddItem(WishItem item);
        Task<bool> AddItem(ToDoItem item);
        Task<bool> AddItem(HouseholdAccountsItem item);
        Task<bool> DeleteItem(WishItem item);
        Task<bool> DeleteItem(ToDoItem item);
        Task<bool> DeleteItem(HouseholdAccountsItem item);
        Task<bool> EditItem(WishItem item);
        Task<bool> EditItem(ToDoItem item);
        Task<bool> EditItem(HouseholdAccountsBalanceItem item);
        Task<bool> EditItem(HouseholdAccountsItem item);
        Task<List<WishItem>> ReadWishList();
        Task<List<ToDoItem>> ReadToDo();
        Task<List<HouseholdAccountsItem>> ReadHouseholdAccounts();
        Task<List<HouseholdAccountsBalanceItem>> ReadBalance();
        

    }
}
