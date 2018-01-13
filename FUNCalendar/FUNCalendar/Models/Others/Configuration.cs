using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace FUNCalendar.Models
{
    public class Configuration
    {
        public bool IsUsedRemoteStorage;

        private readonly string configFileName = "config.json";
        private FileReadWriteService fileReadWriteService= new FileReadWriteService();

        public Configuration ()
        {

        }

        public async Task InitializeAsync()
        {
            if (!await fileReadWriteService.ExistsAsync(configFileName)) await fileReadWriteService.CreateFileAsync(configFileName);
            string configJson = await fileReadWriteService.ReadStringFileAsync(configFileName);
            Configuration configuration = JsonConvert.DeserializeObject<Configuration>(configJson);
            IsUsedRemoteStorage = configuration.IsUsedRemoteStorage;
        }
    }
}
