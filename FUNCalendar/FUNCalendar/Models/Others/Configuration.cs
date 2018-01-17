using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Prism.Mvvm;

namespace FUNCalendar.Models
{
    [JsonObject("config")]
    public class Configuration : BindableBase
    {
        [JsonIgnore]
        private bool isEnableRemoteStorage;
        [JsonProperty("is_enable_remote_storage")]
        public bool IsEnableRemoteStorage
        {
            get { return this.isEnableRemoteStorage; }
            set { this.SetProperty(ref this.isEnableRemoteStorage, value); }
        }
        [JsonIgnore]
        private readonly static string configFileName = "config.json";
        [JsonIgnore]
        private static FileReadWriteService fileReadWriteService;
        [JsonIgnore]
        private string username;
        [JsonProperty("username")]
        public string Username
        {
            get { return this.username; }
            set { this.SetProperty(ref this.username, value); }
        }
        [JsonIgnore]
        private string password;
        [JsonProperty("password")]
        public string Password
        {
            get { return this.password; }
            set { this.SetProperty(ref this.password, value); }
        }

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
