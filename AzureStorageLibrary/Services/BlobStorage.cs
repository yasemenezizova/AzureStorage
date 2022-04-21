using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Specialized;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureStorageLibrary.Services
{
    public class BlobStorage : IBlogStorage
    {
        private readonly BlobServiceClient _blobServiceClient;
        //https://projectbyasaman.blob.core.windows.net/pictures/anders-jilden-cYrMQA7a3Wc-unsplash.jpg
        public BlobStorage()
        {
            _blobServiceClient = new BlobServiceClient(ConnectionStrings.AzureStorageConnectionString);
        }

        public string BlobUrl => "https://projectbyasaman.blob.core.windows.net";

        public async Task DeleteAsync(string fileName, EContainerName eContainerName)
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(eContainerName.ToString());
            var blobClient = containerClient.GetBlobClient(fileName);
            await blobClient.DeleteAsync();

        }

        public async Task<Stream> DownloadAsync(string fileName, EContainerName eContainerName)
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(eContainerName.ToString());
            var blobClient = containerClient.GetBlobClient(fileName);
            var info = await blobClient.DownloadAsync();
            return info.Value.Content;
        }

        public async Task<List<string>> GetLogAsync(string fileName)
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(EContainerName.log.ToString());
            List<string> logs = new List<string>();
            var appendBlobClient = containerClient.GetAppendBlobClient(fileName);
            await appendBlobClient.CreateIfNotExistsAsync();
            var info = await appendBlobClient.DownloadAsync();
            using (StreamReader reader = new StreamReader(info.Value.Content))
            {
                string line = string.Empty;
                while ((line = reader.ReadLine()) != null)
                {
                    logs.Add(line);
                }
            }
            return logs;
        }

        public List<string> GetNames(EContainerName eContainerName)
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(eContainerName.ToString());
            List<string> blobNames = new List<string>();
            var blobs = containerClient.GetBlobs().ToList();
            foreach (var blob in blobs)
            {
                blobNames.Add(blob.Name);
            }
            return blobNames;
        }

        public async Task SetLogAsync(string text, string fileName)
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(EContainerName.log.ToString());
            var appendBlobClient= containerClient.GetAppendBlobClient(fileName);
            await appendBlobClient.CreateIfNotExistsAsync();
            using(MemoryStream ms = new MemoryStream())
            {
                using(StreamWriter writer = new StreamWriter(ms))
                {
                    writer.Write($"{DateTime.Now}: {text}/n"); 
                    writer.Flush();
                    ms.Position = 0;
                    await appendBlobClient.AppendBlockAsync(ms);
                }
            }
        }

        public async Task UploadAsync(Stream fileStream, string fileName, EContainerName eContainerName)
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(eContainerName.ToString());
            await containerClient.CreateIfNotExistsAsync();
            await containerClient.SetAccessPolicyAsync(Azure.Storage.Blobs.Models.PublicAccessType.BlobContainer);
            var blobClient = containerClient.GetBlobClient(fileName);
            await blobClient.UploadAsync(fileStream);

        }
    }
}
