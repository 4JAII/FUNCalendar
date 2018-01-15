using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Text;
using Newtonsoft.Json;
using RestSharp.Portable;
using RestSharp.Portable.Authenticators;
using RestSharp.Portable.HttpClient;
using FUNCalendar.ViewModels;

namespace FUNCalendar.Models
{
    public class DatabaseGateway
    {
        private RestClient restClient;
        private string apiUrl = "https://funcalendar.work/api";

        public DatabaseGateway(string username, string password)
        {
            restClient = new RestClient(apiUrl);
            restClient.Authenticator = new HttpBasicAuthenticator(username, password);
            restClient.IgnoreResponseStatusCode = true;
        }

        private async Task<IRestResponse> GetAsync(string requestPath)
        {
            IRestResponse response;
            RestRequest request = new RestRequest(requestPath, Method.GET);
            request.AddHeader("Accept", "application/json");
            response = await restClient.Execute(request);
            return response;
        }

        private async Task<IRestResponse> PostAsync<T>(string requestPath, T jsonContent)
            where T : class
        {
            IRestResponse response;
            RestRequest request = new RestRequest(requestPath, Method.POST);
            request.AddJsonBody(jsonContent);
            response = await restClient.Execute(request);
            return response;
        }

        private async Task<IRestResponse> PutAsync<T>(string requestPath, int id, T jsonContent)
            where T : class
        {
            IRestResponse response;
            RestRequest request = new RestRequest(requestPath, Method.PUT);
            request.AddUrlSegment("id", id.ToString());        
            request.AddJsonBody(jsonContent);
            response = await restClient.Execute(request);
            return response;
        }

        private async Task<IRestResponse> DeleteAsync(string requestPath, int id)
        {
            IRestResponse response;
            RestRequest request = new RestRequest(requestPath, Method.DELETE);
            request.AddUrlSegment("id", id.ToString());
            response = await restClient.Execute(request);
            return response;

        }

        public async Task<List<WishItem>> GetWishListAsync()
        {
            string requestPath = "v1/wishlist";
            IRestResponse response;
            List<WishItem> list;
            response = await GetAsync(requestPath);
            if (response.StatusCode != HttpStatusCode.OK)
                return null;
            list = JsonConvert.DeserializeObject<JsonWishList>(response.Content).Value;
            return list;
        }

        public async Task<Tuple<int, int>> PostWishListAsync(WishItem wishItem)
        {
            JsonWishItem jsonWishItem = new JsonWishItem { VMValue = new VMWishItem(wishItem) };
            string requestPath = "v1/wishlist";
            IRestResponse response = await PostAsync<JsonWishItem>(requestPath, jsonWishItem);
            JsonStatus status = JsonConvert.DeserializeObject<JsonStatus>(response.Content);
            return Tuple.Create((int)response.StatusCode, status.LastAddedID);
        }

        public async Task<int> PutWishListAsync(WishItem wishItem)
        {
            JsonWishItem jsonWishItem = new JsonWishItem { VMValue = new VMWishItem(wishItem) };
            string requestPath = "v1/wishlist/{id}";
            IRestResponse response = await PutAsync<JsonWishItem>(requestPath, wishItem.ID, jsonWishItem);
            return (int)response.StatusCode;
        }

        public async Task<int> DeleteWishListAsync(WishItem wishItem)
        {
            string requestPath = "v1/wishlist/{id}";
            IRestResponse response = await DeleteAsync(requestPath, wishItem.ID);
            return (int)response.StatusCode;
        }


    }
}
