using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace FUNCalendar.Models
{
    public class JsonStatus
    {
        private class Status
        {
            [JsonProperty("code")]
            public int Code { get; private set; }
            [JsonProperty("message")]
            public string Message { get; private set; }
            [JsonProperty("last_added_id")]
            public int LastAddedID { get; private set; }
        }

        private Status statusValue;
        [JsonProperty("status")]
        private Status status
        {
            set
            {
                statusValue = value;
                this.Code = statusValue.Code;
                this.Message = statusValue.Message;
                this.LastAddedID = statusValue.LastAddedID;
            }
        }

        public int Code { get; private set; }
        
        public string Message { get; private set; }
        
        public int LastAddedID { get; private set; }
    }
}
