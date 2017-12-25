using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FUNCalendar.Services;

namespace FUNCalendar.Models.Others
{
    class RemoteStorage : IStorageService
    {
        public int LastAddedWishItemID { get; private set; }

        public int LastAddedToDoItemID { get; private set; }

        public int LastAddedHouseHoldAccountsID { get; private set; }

        public Task<bool> AddItem(WishItem item)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteItem(WishItem item)
        {
            throw new NotImplementedException();
        }

        public Task<bool> EditItem(WishItem item)
        {
            throw new NotImplementedException();
        }

        public Task<List<WishItem>> ReadFile()
        {
            throw new NotImplementedException();
        }
    }
}
