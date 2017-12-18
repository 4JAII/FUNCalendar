using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FUNCalendar.Models
{
    public enum Balancetype
    {
        incomes = 1,
        outgoings,
        difference
    } 
    public class HouseholdaccountStatisticItem
    {
        public Balancetype Bt { get; set; }
        public int Price { get; set; }

        public HouseholdaccountStatisticItem(Balancetype bc,int price)
        {
            this.Bt = bc;
            this.Price = price;
        }
    }
}
