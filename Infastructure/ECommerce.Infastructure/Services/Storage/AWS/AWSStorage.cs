using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using ECommerce.Application.Abstraction.Storage.AWS;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Net.Http.Headers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Infastructure.Services.Storage.AWS
{
    public class AWSStorage : Storage, IAWSStorage
    {
        private readonly string bucketName;
        private readonly IAmazonS3 _amazonS3;

        public AWSStorage(IAmazonS3 amazonS3, IConfiguration configuration)
        {
            _amazonS3 = amazonS3;
            bucketName = configuration["Storage:AWS:AWSBucketName"];
        }

        public async Task DeleteAsync(string container, string fileName)
        {
            var s3Client = new AmazonS3Client(RegionEndpoint.EUCentral1);

            string fileKey = $"{container}/{fileName}";

            var deleteRequest = new DeleteObjectRequest
            {
                BucketName = bucketName,
                Key = fileKey
            };

            var response = await s3Client.DeleteObjectAsync(deleteRequest);
        }

        public List<string> GetFiles(string container)
        {
            var s3Client = new AmazonS3Client();
            var response = s3Client.ListObjectsAsync(bucketName, container).Result;
            return response.S3Objects.Select(a => a.Key).ToList();
        }

        public bool HasFile(string container, string fileName)
        {
            var s3Client = new AmazonS3Client();
            var response = s3Client.ListObjectsAsync(bucketName, container).Result;
            return response.S3Objects.Any(a => a.Key == fileName);
        }

        public async Task<List<(string fileName, string pathOrContrainer)>> uploadAsync(string container, IFormFileCollection files)
        {
            List<(string fileName, string pathOrContrainer)> datas = new();

            foreach (IFormFile file in files)
            {
                using (var memoryStream = new MemoryStream())
                {

                    await file.CopyToAsync(memoryStream);
                    string fileName = FileRename(file.Name);
                    string fileKey = $"{container}/{fileName}";
                    var putRequest = new Amazon.S3.Model.PutObjectRequest
                    {
                        BucketName = bucketName,
                        Key = fileKey,
                        InputStream = memoryStream,
                        CannedACL = S3CannedACL.PublicRead
                    };

                    var response = await _amazonS3.PutObjectAsync(putRequest);
                    if (response.HttpStatusCode == System.Net.HttpStatusCode.OK)
                    {
                        datas.Add((fileName, $"{container}/{fileName}"));
                    }
                }

            }
            return datas;
        }
    }
}
