using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FUNCalendar.Models
{
    public class HouseholdAccountsDcStatisticsItem
    {
        public BalanceTypes BalanceType { get; set; }
        public SCategorys Scategory { set; get; }
        public DCategorys Dcategory { get; set; }
        public int Price { get; set; }
        public int Ratio { get; set; }

        public HouseholdAccountsDcStatisticsItem(BalanceTypes balancetype, SCategorys scategory,DCategorys dcategory, int price, int ratio){
            this.BalanceType = balancetype;
            this.Scategory = scategory;
            this.Dcategory = dcategory;
            this.Price = price;
            this.Ratio = ratio;
        }
    }
}
