using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoTHubEventProcessor.Helper
{
    public class BlobStorageHelper
    {
        private CloudBlobContainer _container;

        public BlobStorageHelper(string storageConnectionString, string storageContainer)
        {
            try
            {
                CloudStorageAccount storageAccount = CloudStorageAccount.Parse(storageConnectionString);
                CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

                _container = blobClient.GetContainerReference(storageContainer);
                _container.CreateIfNotExistsAsync();

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }
        public async void Save(string blobName, string data)
        {
            try
            {
                CloudBlockBlob blockBlob = _container.GetBlockBlobReference(blobName);
                await blockBlob.UploadTextAsync(data);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }
    }
}
