using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FUNCalendar.Models;

namespace FUNCalendar.Services
{
    public interface IStorageService
    {
        Configuration Config { get; }
        IWishList WishList { get; }
        bool HasError { get; }
        Task InitializeAsync(IWishList wishList, IToDoList todoList, IHouseholdAccounts householdAccounts);

        void AfterResolveError();
        Task SetConfig(bool isEnableRemoteStorage, string username, string password);
        Task AddItem(WishItem item);
        Task AddItem(ToDoItem item);
        Task AddItem(HouseholdAccountsItem item);
        Task DeleteItem(WishItem item);
        Task DeleteItem(ToDoItem item);
        Task DeleteItem(HouseholdAccountsItem item);
        Task EditItem(WishItem deleteItem, WishItem addItem);
        Task EditItem(ToDoItem deleteItem,ToDoItem addItem);
        Task EditItem(HouseholdAccountsItem deleteItem,HouseholdAccountsItem addItem);
        //Task EditItem(HouseholdAccountsBalanceItem Item, int Price);
        Task ReadFile();
    }
}
