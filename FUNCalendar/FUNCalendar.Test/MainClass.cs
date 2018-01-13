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
            //List<WishItem> list;
            //list = await databaseGateway.GetWishListAsync();
            //string jsonWishList = JsonConvert.SerializeObject(new JsonWishList { VMValue = list.Select(x => new VMWishItem(x)).ToList() },Formatting.Indented);
            int temp = await databaseGateway.DeleteWishListAsync(new WishItem {ID = 17, Name = "更新アイテム", Price = 14514, Date = DateTime.Now, IsBought = false, ToDoID = 0 });
            Console.WriteLine("HTTP_STATUS_CODE:"+temp);
            
        }

        public static void Main()
        {
            MainBody().Wait();
            Console.Read();
        }
    }
}
