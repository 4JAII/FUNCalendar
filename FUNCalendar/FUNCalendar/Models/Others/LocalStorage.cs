using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PCLStorage;
using Prism.Mvvm;
using SQLitePCL;
using FUNCalendar.Services;
using SQLite;

namespace FUNCalendar.Models
{
    public class LocalStorage : IStorage
    {
        private IFolder rootFolder;
        private readonly string databaseFileName = "FUNCalendarDB.db";
        private FileReadWriteService fileReadWriteService;
        private static SQLiteAsyncConnection asyncConnection;
        private static bool isInitialized = false;

        /* 最後に追加したIDを保持 */
        public int LastAddedWishItemID { get; private set; }
        public int LastAddedToDoItemID { get; private set; }
        public int LastAddedHouseholdAccountsItemID { get; private set; }
        public int LastAddedBalanceItemID { get; private set; }

        private async Task CreateConnection()
        {
            if (isInitialized) return;
            IFile file;
            SQLiteAsyncConnection connection;
            rootFolder = FileSystem.Current.LocalStorage;
            fileReadWriteService = new FileReadWriteService();

            var result = await fileReadWriteService.ExistsAsync(databaseFileName).ConfigureAwait(false);
            if (!result)
            {
                await fileReadWriteService.CreateFileAsync(databaseFileName);
                file = await fileReadWriteService.ReadFileAsync(databaseFileName);
            }
            file = await fileReadWriteService.ReadFileAsync(databaseFileName);
            connection = new SQLiteAsyncConnection(file.Path);
            await connection.CreateTableAsync<WishItem>();
            await connection.CreateTableAsync<ToDoItem>();
            await connection.CreateTableAsync<HouseholdAccountsItem>();
            await connection.CreateTableAsync<HouseholdAccountsBalanceItem>();
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
            return true;
        }



        public async Task<bool> AddItem(HouseholdAccountsBalanceItem item)
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
            LastAddedBalanceItemID = item.ID;
            return true;
        }



        public async Task<bool> AddItem(HouseholdAccountsItem item)
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
            LastAddedHouseholdAccountsItemID = item.ID;
            return true;
        }


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



        public async Task<bool> DeleteItem(HouseholdAccountsItem item)
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

        public async Task<bool> EditItem(HouseholdAccountsBalanceItem item)
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

        public async Task<bool> EditItem(HouseholdAccountsItem item)
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


        public async Task<List<WishItem>> ReadWishList()
        {
            await CreateConnection();
            return await asyncConnection.Table<WishItem>().ToListAsync();
        }


        public async Task<List<ToDoItem>> ReadToDo()
        {
            await CreateConnection();
            return await asyncConnection.Table<ToDoItem>().ToListAsync();
        }



        public async Task<List<HouseholdAccountsBalanceItem>> ReadBalance()
        {
            await CreateConnection();
            return await asyncConnection.Table<HouseholdAccountsBalanceItem>().ToListAsync();
        }



        public async Task<List<HouseholdAccountsItem>> ReadHouseholdAccounts()
        {
            await CreateConnection();
            return await asyncConnection.Table<HouseholdAccountsItem>().ToListAsync();
        }


    }
}
