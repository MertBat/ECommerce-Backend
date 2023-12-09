using ECommerce.Application.Abstraction.Storage;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Infastructure.Services.Storage
{
    public class StorageService : IStorageService
    {
        private readonly IStorage _storage;

        public StorageService(IStorage storage)
        {
            _storage = storage;
        }

        public string StorageName { get => _storage.GetType().Name; }

        public async Task DeleteAsync(string pathOrContrainer, string fileName)
        {
            await _storage.DeleteAsync(pathOrContrainer, fileName);
        }

        public  List<string> GetFiles(string pathOrContrainer)
        {
            return _storage.GetFiles(pathOrContrainer);
        }

        public bool HasFile(string pathOrContrainer, string fileName)
        {
            return _storage.HasFile(pathOrContrainer, fileName);
        }

        public Task<List<(string fileName, string pathOrContrainer)>> uploadAsync(string pathOrContrainer, IFormFileCollection files)
        {
            return _storage.uploadAsync(pathOrContrainer, files);
        }
    }
}
