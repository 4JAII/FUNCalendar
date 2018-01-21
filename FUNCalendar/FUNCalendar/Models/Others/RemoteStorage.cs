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

        public int LastAddedHouseholdAccountsItemID { get; private set; }

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
                return new List<WishItem>();
            }
        }

        public async Task <bool> AddItem(ToDoItem item)
        {
            Tuple<int, int> result;
            try
            {
                result = await gateway.PostToDoAsync(item);
            }
            catch
            {
                return false;
            }
            if(result.Item1 != (int)HttpStatusCode.Created)
            {
                LastAddedToDoItemID = 0;
                return false;
            }
            LastAddedToDoItemID = result.Item2;
            return true;
        }

        public async Task<bool> DeleteItem(ToDoItem item)
        {
            int result;
            try
            {
                result = await gateway.DeleteToDoAsync(item);
            }
            catch
            {
                return false;
            }
            if(result != (int)HttpStatusCode.OK)
                return false;
            return true;
        }

        public async Task<bool> EditItem(ToDoItem item)
        {
            int result;
            try
            {
                result = await gateway.PutToDoAsync(item);
            }
            catch
            {
                return false;
            }
            if(result != (int)HttpStatusCode.OK)
                return false;
            return true;
        }

        public async Task<List<ToDoItem>> ReadToDo()
        {
            try
            {
                return await gateway.GetToDoAsync();
            }
            catch
            {
                return new List<ToDoItem>();
            }

        }

        public async Task<bool> AddItem(HouseholdAccountsItem item)
        {
            Tuple<int,int> result;
            try
            {
                result = await gateway.PostHouseholdAccountsAsync(item);
            }
            catch
            {
                return false;
            }
            if(result.Item1 !=(int)HttpStatusCode.Created)
            {
                LastAddedHouseholdAccountsItemID = 0;
                return false;
            }
            LastAddedHouseholdAccountsItemID = result.Item2;
            return true;
        }

        public async Task<bool> DeleteItem(HouseholdAccountsItem item)
        {
            int result;
            try
            {
                result = await gateway.DeleteHouseholdAccountsAsync(item);
            }
            catch
            {
                return false;
            }
            if(result != (int)HttpStatusCode.OK)
                return false;
            return true;
        }

        public async Task<bool> EditItem(HouseholdAccountsItem item)
        {
            int result;
            try
            {
                result = await gateway.PutHouseholdAccountsAsync(item);
            }
            catch
            {
                return false;
            }
            if(result != (int)HttpStatusCode.OK)
                return false;
            return true;
        }

        public async Task<List<HouseholdAccountsItem>> ReadHouseholdAccounts()
        {
            try
            {
                return await gateway.GetHouseholdAccountsAsync();
            }
            catch
            {
                return new List<HouseholdAccountsItem>();
            }
        }
        /*
        public async Task<bool> EditItem(HouseholdAccountsBalanceItem item)
        {
            int result;
            try
            {
                result = await gateway.PutBalanceItemAsync(item);
            }
            catch
            {
                return false;
            }
            if(result !=(int)HttpStatusCode.OK)
                return false;
            return true;
        }
        
        public async Task<List<HouseholdAccountsBalanceItem>> ReadBalance()
        {
            try
            {
                return await gateway.GetBalanceAsync();
            }
            catch
            {
                return null;
            }
        }
        */
    }
}
