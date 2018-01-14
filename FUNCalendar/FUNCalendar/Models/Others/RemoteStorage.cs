using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Threading.Tasks;
using FUNCalendar.Services;
using RestSharp;
using RestSharp.Portable;

namespace FUNCalendar.Models
{
    public class RemoteStorage : IStorage
    {
        public int LastAddedWishItemID { get; private set; }

        public int LastAddedToDoItemID { get; private set; }

        public int LastAddedHouseholdAccountsID { get; private set; }

        private DatabaseGateway gateway;

        public RemoteStorage(string username, string password)
        {
            gateway = new DatabaseGateway(username, password);
        }

        public RemoteStorage()
        {

        }

        public async Task<bool> AddItem(WishItem item)
        {
            Tuple<int, int> result;
            try
            {
                result = await gateway.PostWishListAsync(item);
            }
            catch
            {
                return false;
            }

            if (result.Item1 != (int)HttpStatusCode.Created)
            {
                LastAddedWishItemID = 0;
                return false;
            }
            LastAddedWishItemID = result.Item2;
            return true;
        }

        public async Task<bool> DeleteItem(WishItem item)
        {
            int result;
            try
            {
                result = await gateway.DeleteWishListAsync(item);
            }
            catch
            {
                return false;
            }
            if (result != (int)HttpStatusCode.OK)
                return false;
            return true;
        }

        public async Task<bool> EditItem(WishItem item)
        {
            int result;
            try
            {
                result = await gateway.PutWishListAsync(item);
            }
            catch
            {
                return false;
            }
            if (result != (int)HttpStatusCode.OK)
                return false;
            return true;
        }

        public async Task<List<WishItem>> ReadWishList()
        {
            try
            {
                return await gateway.GetWishListAsync();

            }
            catch
            {
                return null;
            }
        }
    }
}
