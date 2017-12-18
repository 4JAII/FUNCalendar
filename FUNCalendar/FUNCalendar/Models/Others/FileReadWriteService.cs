using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PCLStorage;

namespace FUNCalendar.Models
{
    public class FileReadWriteService
    {
        private IFolder rootFolder = FileSystem.Current.LocalStorage;
       
        private async Task<IFile> LoadFileAsync(string fileName)
        {
            ExistenceCheckResult res = await rootFolder.CheckExistsAsync(fileName);
            if (res == ExistenceCheckResult.FileExists) return await rootFolder.GetFileAsync(fileName);
            return await rootFolder.CreateFileAsync(fileName, CreationCollisionOption.ReplaceExisting);
        }

        public async Task<IFile> ReadFileAsync(string fileName)
        {
            return await LoadFileAsync(fileName);
        }

        public async Task<string> ReadStringFileAsync(string fileName)
        {
            IFile file = await LoadFileAsync(fileName);
            return await file.ReadAllTextAsync();
        }

        public async Task WriteStringFileAsync(string fileName,string contents)
        {
            IFile file = await LoadFileAsync(fileName);
            await file.WriteAllTextAsync(contents);
        }

        public async Task<bool> ExistsAsync(string fileName)
        {
            ExistenceCheckResult result = await rootFolder.CheckExistsAsync(fileName);
            if (result == ExistenceCheckResult.FileExists) return true;
            return false;
        }

        public async Task CreateFileAsync(string fileName)
        {
            await rootFolder.CreateFileAsync(fileName, CreationCollisionOption.ReplaceExisting);
        }
    }
}
