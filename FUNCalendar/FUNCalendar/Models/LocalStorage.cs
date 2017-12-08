using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PCLStorage;
using Prism.Mvvm;
using SQLite;


namespace FUNCalendar.Models
{
    public class LocalStorage : BindableBase
    {
        private IFolder rootFolder;
        private readonly string databaseFileName = "FUNCalendarDB.db";
        private FileReadWriteService fileReadWriteService;
        private static SQLiteAsyncConnection asyncConnection;
        private static bool isInitialized = false;

        public int LastAddedWishItemID { get; private set; }


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
                /* 他のアイテムも追加 */

            }
            file = await fileReadWriteService.ReadFileAsync(databaseFileName);
            connection = new SQLiteAsyncConnection(file.Path);
            asyncConnection = connection;
            isInitialized = true;
        }

        /* 各セーブ処理 */
        public async Task AddItem(WishItem item)
        {
            await CreateConnection();
            await asyncConnection.InsertAsync(item);
            LastAddedWishItemID = item.ID;
        }

        public async Task DeleteItem(WishItem item)
        {
            await CreateConnection();
            await asyncConnection.DeleteAsync(item);
        }

        public async Task EditItem(WishItem item)
        {
            await CreateConnection();
            await asyncConnection.UpdateAsync(item);
        }

        public async Task<List<WishItem>> ReadFile()
        {
            await CreateConnection();
            return await asyncConnection.Table<WishItem>().ToListAsync();
        }

    }
}
