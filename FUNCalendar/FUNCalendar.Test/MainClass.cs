using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FUNCalendar.Models;
using FUNCalendar.ViewModels;
using Newtonsoft.Json;


namespace FUNCalendar.Test
{
    class MainClass
    {

        public static async Task MainBody()
        {
            DatabaseGateway databaseGateway = new DatabaseGateway("gunma", "akagi");
            List<WishItem> list;
            list = await databaseGateway.GetWishListAsync();
            string jsonWishList = JsonConvert.SerializeObject(new JsonWishList { VMValue = list.Select(x => new VMWishItem(x)).ToList() },Formatting.Indented);
            
            Console.WriteLine(jsonWishList);
            
        }

        public static void Main()
        {
            MainBody().Wait();
            Console.Read();
        }
    }
}
