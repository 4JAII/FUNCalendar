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
        Task AddItem(WishItem item,bool needsRegister,int priority);
        Task AddItem(ToDoItem item);
        Task AddItem(HouseholdAccountsItem item);
        Task DeleteItem(WishItem item,bool needsDelete);
        Task DeleteItem(ToDoItem item,bool needsDelete);
        Task DeleteItem(HouseholdAccountsItem item);
        Task EditItem(WishItem deleteItem, WishItem addItem,bool needsRegister,int priority);
        Task EditItem(ToDoItem deleteItem,ToDoItem addItem);
        Task EditItem(HouseholdAccountsItem deleteItem,HouseholdAccountsItem addItem);
        Task ReadFile();
        Task CompleteToDo(ToDoItem newTodoItem, bool hasId, bool needsRegister, SCategorys Scategory, DCategorys Dcategory, StorageTypes storagetype);
    }
}
