using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using ECommerce.Application.Abstraction.Storage.Azure;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System.ComponentModel;
using System.Xml.Linq;

namespace ECommerce.Infastructure.Services.Storage.Azure
{
    public class AzureStorage : Storage, IAzureStorage
    {
        readonly BlobServiceClient _blobServiceClient;
        BlobContainerClient _blobContainerClient;

        public AzureStorage(IConfiguration configuration)
        {
            _blobServiceClient = new(configuration["Storage:Azure"]);
        }

        public async Task DeleteAsync(string contrainer, string fileName)
        {
            _blobContainerClient = _blobServiceClient.GetBlobContainerClient(contrainer);
            BlobClient blob = _blobContainerClient.GetBlobClient(fileName);
            await blob.DeleteAsync();
        }

        public List<string> GetFiles(string contrainer)
        {
            _blobContainerClient = _blobServiceClient.GetBlobContainerClient(contrainer);
            return _blobContainerClient.GetBlobs().Select(x => x.Name).ToList();
        }

        public bool HasFile(string contrainer, string fileName)
        {
            _blobContainerClient = _blobServiceClient.GetBlobContainerClient(contrainer);
            return _blobContainerClient.GetBlobs().Any(x => x.Name == fileName);
        }

        public async Task<List<(string fileName, string pathOrContrainer)>> uploadAsync(string contrainer, IFormFileCollection files)
        {
            _blobContainerClient = _blobServiceClient.GetBlobContainerClient(contrainer);
            await _blobContainerClient.CreateIfNotExistsAsync();
            //await _blobContainerClient.SetAccessPolicyAsync(PublicAccessType.BlobContainer);

            List<(string fileName, string pathOrContrainer)> datas = new();
            foreach (IFormFile file in files)
            {
                string newFileName = FileRename(file.Name);
                BlobClient blobClient = _blobContainerClient.GetBlobClient(newFileName);
                await blobClient.UploadAsync(file.OpenReadStream());
                datas.Add((newFileName, $"{contrainer}/{newFileName}"));
            }

            return datas;
        }
    }
}
