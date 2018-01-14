using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace FUNCalendar.Models
{
    [JsonObject("config")]
    public class Configuration
    {
        [JsonProperty("is_used_remote_storage")]
        public bool IsEnableRemoteStorage { get; private set; }
        [JsonIgnore]
        private readonly static string configFileName = "config.json";
        [JsonIgnore]
        private static FileReadWriteService fileReadWriteService;
        [JsonProperty("username")]
        public string Username { get; set; }
        [JsonProperty("password")]
        public string Password { get; set; }
        [JsonIgnore]
        private bool isInitialized = false;

        public Configuration()
        {

        }

        public static async Task<Configuration> InitializeAsync()
        {
            fileReadWriteService = new FileReadWriteService();
            if (!await fileReadWriteService.ExistsAsync(configFileName))
            {
                await fileReadWriteService.CreateFileAsync(configFileName);
                var temp = new Configuration();
                string config = JsonConvert.SerializeObject(temp);
                await fileReadWriteService.WriteStringFileAsync(configFileName, config);
            }
            string configJson = await fileReadWriteService.ReadStringFileAsync(configFileName);
            Configuration configuration = JsonConvert.DeserializeObject<Configuration>(configJson);
            return configuration;
        }


        public async Task WriteFile()
        {
            string configJson = JsonConvert.SerializeObject(this);
            await fileReadWriteService.WriteStringFileAsync(configFileName, configJson);
        }
    }
}
