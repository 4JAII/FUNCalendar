using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FUNCalendar.Services;
using RestSharp;
using RestSharp.Portable;

namespace FUNCalendar.Models
{
    public class RemoteStorage : IStorageService
    {
        public int LastAddedWishItemID { get; private set; }

        public int LastAddedToDoItemID { get; private set; }

        public int LastAddedHouseholdAccountsID { get; private set; }

        public async Task<bool> AddItem(WishItem item)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> DeleteItem(WishItem item)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> EditItem(WishItem item)
        {
            throw new NotImplementedException();
        }

        public async Task<List<WishItem>> ReadFile()
        {
            throw new NotImplementedException();
        }
    }
}
