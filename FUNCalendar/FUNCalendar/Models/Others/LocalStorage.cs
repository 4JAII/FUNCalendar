using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PCLStorage;
using Prism.Mvvm;
using SQLite;
using FUNCalendar.Services;


namespace FUNCalendar.Models
{
    public class LocalStorage : IStorageService
    {
        private IFolder rootFolder;
        private readonly string databaseFileName = "FUNCalendarDB.db";
        private FileReadWriteService fileReadWriteService;
        private static SQLiteAsyncConnection asyncConnection;
        private static bool isInitialized = false;

        /* 最後に追加したIDを保持 */
        public int LastAddedWishItemID { get; private set; }
        public int LastAddedToDoItemID { get; private set; }
        public int LastAddedHouseHoldAccountsID { get; private set; }

        public LocalStorage()
        {
            rootFolder = FileSystem.Current.LocalStorage;
            fileReadWriteService = new FileReadWriteService();
        }

        private async Task CreateConnection()
        {
            if (isInitialized) return;
            IFile file;
            SQLiteAsyncConnection connection;
            var result = await fileReadWriteService.ExistsAsync(databaseFileName).ConfigureAwait(false);
            if (!result)
            {
                await fileReadWriteService.CreateFileAsync(databaseFileName);
                file = await fileReadWriteService.ReadFileAsync(databaseFileName);
                connection = new SQLiteAsyncConnection(file.Path);
                await connection.CreateTableAsync<WishItem>();
              /*await connection.CreateTableAsync<ToDoItem>();   
                await connection.CreateTableAsync<HouseHoldAccountsItem>();
                */

            }
            file = await fileReadWriteService.ReadFileAsync(databaseFileName);
            connection = new SQLiteAsyncConnection(file.Path);
            asyncConnection = connection;
            isInitialized = true;
        }

        /* 各セーブ処理 */
        public async Task<bool> AddItem(WishItem item)
        {
            try
            {
                await CreateConnection();
                await asyncConnection.InsertAsync(item);
            }
            catch
            {
                return false;
            }
            LastAddedWishItemID = item.ID;
            return true;
        }

        /*
        public async Task<bool> AddItem(ToDoItem item)
        {
            try
            {
                await CreateConnection();
                await asyncConnection.InsertAsync(item);
            }
            catch
            {
                return false;
            }
            LastAddedToDoItemID = item.ID;
        }
        */

        /*
        public async Task<bool> AddItem(BalanceItem item)
        {
            try
            {
                await CreateConnection();
                await asyncConnection.InsertAsync(item);
            }
            catch
            {
                return false;
            }
        }
        */

        /*
        public async Task<bool> AddItem(HouseHoldAccountsItem item)
        {
            try
            {
                await CreateConnection();
                await asyncConnection.InsertAsync(item);
            }
            catch
            {
                return false;
            }
            LastAddedHouseHoldAccountsItemID = item.ID;
        }
        */

        public async Task<bool> DeleteItem(WishItem item)
        {
            try
            {
                await CreateConnection();
                await asyncConnection.DeleteAsync(item);
            }
            catch
            {
                return false;
            }
            return true;
        }

        /*
        public async Task<bool> DeleteItem(ToDoItem item)
        {
            try
            {
                await CreateConnection();
                await asyncConnection.DeleteAsync(item);
            }
            catch
            {
                return false;
            }
            return true;
        }
        */

        /*
        public async Task<bool> DeleteItem(HouseHoldAccounts item)
        {
            try
            {
                await CreateConnection();
                await asyncConnection.DeleteAsync(item);
            }
            catch
            {
                return false;
            }
            return true;
        }
        */

        public async Task<bool> EditItem(WishItem item)
        {
            try
            {
                await CreateConnection();
                await asyncConnection.UpdateAsync(item);
            }
            catch
            {
                return false;
            }
            return true;
        }

        /*
        public async Task<bool> EditItem(ToDoItem item)
        {
           try
            {
                await CreateConnection();
                await asyncConnection.UpdateAsync(item);
            }
            catch
            {
                return false;
            }
            return true;
        }
        */

        /*
        public async Task<bool> EditItem(HouseHoldAccountsItem item)
        {
           try
            {
                await CreateConnection();
                await asyncConnection.UpdateAsync(item);
            }
            catch
            {
                return false;
            }
            return true;
        }
        */

        public async Task<List<WishItem>> ReadFile()
        {
            await CreateConnection();
            return await asyncConnection.Table<WishItem>().ToListAsync();
        }

        /*
        public async Task<List<ToDoItem>> ReadFile()
        {
            await CreateConnection();
            return await asyncConnection.Table<ToDoItem>().ToListAsync();
        }
        */

        /*
        public async Task<List<BalanceItem>> ReadFile()
        {
            await CreateConnection();
            return await asyncConnection.Table<BalanceItem>().ToListAsync();
        }
        */

        /*
        public async Task<List<HouseHoldAccountsItem>> ReadFile()
        {
            await CreateConnection();
            return await asyncConnection.Table<HouseHoldAccountsItem>().ToListAsync();
        }
        */

    }
}
