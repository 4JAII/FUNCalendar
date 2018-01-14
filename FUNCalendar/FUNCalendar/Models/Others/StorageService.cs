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

        public StorageService()
        {
            WishList = new WishList();
            /*他のやつも*/
        }

        public async Task InitializeAsync()
        {
            if (isInitialized) return;
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

        public async Task EditItem(WishItem deleteItem, WishItem addItem)
        {
            if (!isInitialized)
            {
                HasError = true;
                return;
            }
            HasError = !await storage.EditItem(addItem);
            WishList.EditWishItem(deleteItem,addItem);
        }

        public async Task ReadFile()
        {
            if (!isInitialized)
            {
                HasError = true;
                return;
            }
            
            WishList.InitializeList(await storage.ReadWishList());
            /*ここに他のやつも*/
        }
    }
}
