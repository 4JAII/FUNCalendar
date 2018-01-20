using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FUNCalendar.Services;
using FUNCalendar.Models;

namespace FUNCalendar.Models
{
    public class StorageService : IStorageService
    {
        private bool isInitialized = false;
        public bool HasError { get; private set; }
        private IStorage storage;

        public Configuration Config { get; private set; }
        public IWishList WishList { get; private set; }
        public IToDoList ToDoList { get; private set; }
        public IHouseholdAccounts HouseholdAccounts{ get; private set;}

        public StorageService()
        {

        }

        public async Task InitializeAsync(IWishList wishList,IToDoList todoList,IHouseholdAccounts householdAccounts)
        {
            if (isInitialized) return;
            WishList = wishList;
            ToDoList = todoList;
            HouseholdAccounts = householdAccounts;
            Config = await Configuration.InitializeAsync();
            if (Config.IsEnableRemoteStorage)
                storage = new RemoteStorage(Config.Username, Config.Password);
            else
                storage = new LocalStorage();
            isInitialized = true;
        }

        /* 問題を解決したあとのコールバック */
        public void AfterResolveError()
        {
            HasError = false;
        }

        public async Task SetConfig(bool isEnableRemoteStorage, string username, string password)
        {
            Config.IsEnableRemoteStorage = isEnableRemoteStorage;
            Config.Username = username;
            Config.Password = password;
            if (Config.IsEnableRemoteStorage)
                storage = new RemoteStorage(Config.Username, Config.Password);
            else
                storage = new LocalStorage();
            await ReadFile();
            await Config.WriteFile();
        }

        public async Task AddItem(WishItem item)
        {
            if (!isInitialized)
            {
                HasError = true;
                return;
            }
            HasError = !await storage.AddItem(item);
            item.ID = storage.LastAddedWishItemID;
            WishList.AddWishItem(item);
            /*ToDoID == 0以外でwishitem を変換しaddメソッド*/
        }

        public async Task AddItem(ToDoItem item)
        {
            if (!isInitialized)
            {
                HasError = true;
                return;
            }
            HasError = !await storage.AddItem(item);
            item.ID = storage.LastAddedToDoItemID;
            ToDoList.AddToDoItem(item);
        }

        public async Task AddItem(HouseholdAccountsItem item)
        {
            if (!isInitialized)
            {
                HasError = true;
                return;
            }
            HasError = !await storage.AddItem(item);
            item.ID = storage.LastAddedHouseholdAccountsItemID;
            HouseholdAccounts.AddHouseholdAccountsItem(item);
        }



        public async Task DeleteItem(WishItem item)
        {
            if (!isInitialized)
            {
                HasError = true;
                return;
            }
            HasError = !await storage.DeleteItem(item);
            WishList.Remove(item);
            /*ToDoID == 0以外でwishitem を変換しaddメソッド*/
        }

        public async Task DeleteItem(ToDoItem item)
        {
            if (!isInitialized)
            {
                HasError = true;
                return;
            }
            HasError = !await storage.DeleteItem(item);
            ToDoList.Remove(item);
        }

        public async Task DeleteItem(HouseholdAccountsItem item)
        {
            if (!isInitialized)
            {
                HasError = true;
                return;
            }
            HasError = !await storage.DeleteItem(item);
            HouseholdAccounts.RemoveHouseholdAccountsItem(item);
        }

        public async Task EditItem(WishItem deleteItem, WishItem addItem)
        {
            if (!isInitialized)
            {
                HasError = true;
                return;
            }
            HasError = !await storage.EditItem(addItem);
            WishList.EditWishItem(deleteItem, addItem);
            //EditItem(todo)
        }

        public async Task EditItem(ToDoItem deleteItem,ToDoItem addItem)
        {
            if (!isInitialized)
            {
                HasError = true;
                return;
            }
            HasError = !await storage.EditItem(addItem);
            ToDoList.EditToDoItem(deleteItem, addItem);
        }

        public async Task EditItem(HouseholdAccountsItem deleteItem,HouseholdAccountsItem addItem)
        {
            if (!isInitialized)
            {
                HasError = true;
                return;
            }
            HasError = !await storage.EditItem(addItem);
            HouseholdAccounts.EditHouseholdAccountsItem(deleteItem, addItem);
        }
        /*
        public async Task EditItem(HouseholdAccountsBalanceItem deleteItem,HouseholdAccountsBalanceItem addItem)
        {
            if (!isInitialized)
            {
                HasError = true;
                return;
            }
            HasError = !await storage.EditItem(addItem);
           
        }
        
         public async Task EditItem(HouseholdAccountsBalanceItem item, int price)
         {
            if (!isInitialized)
            {
                HasError = true;
                return;
            }
            HasError = !await storage.EditItem(item);
            HouseholdAccounts.EditHouseholdAccountsBalance(item, price);
         }
         */

        public async Task ReadFile()
        {
            if (!isInitialized)
            {
                HasError = true;
                return;
            }

            WishList.UpdateList(await storage.ReadWishList());
            ToDoList.UpdateList(await storage.ReadToDo());
            HouseholdAccounts.UpdateList(await storage.ReadHouseholdAccounts());
        }

        public void CompleteToDo(ToDoItem todoItem, bool hasId, bool needsResister)
        {
            /* todoを完了にする */
            /* 対応するIDのwishitemを購入済みにする hasId == true*/
            /* wishitem -> householdaccountsitemに変換し保存 needsResister == true*/
        }


    }
}
