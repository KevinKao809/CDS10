using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Microsoft.Azure; // Namespace for CloudConfigurationManager
using Microsoft.WindowsAzure.Storage; // Namespace for CloudStorageAccount
using Microsoft.WindowsAzure.Storage.Blob; // Namespace for Blob storage types
using System.IO;

namespace sfAPIService.Models
{
    public class SharedFunctions
    {
        public string SaveFiletoStorage(string localFileName, string uploadFilePath, string containerName)
        {
            // Parse the connection string and return a reference to the storage account.
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("StorageConnectionString"));
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

            // Retrieve a reference to a container.
            CloudBlobContainer container = blobClient.GetContainerReference(containerName);
            container.SetPermissions(new BlobContainerPermissions { PublicAccess = BlobContainerPublicAccessType.Blob });

            // Create the container if it doesn't already exist.
            container.CreateIfNotExists();

            CloudBlockBlob blockBlob = container.GetBlockBlobReference(uploadFilePath);
            // Create or overwrite the "myblob" blob with contents from a local file.
            using (var fileStream = File.OpenRead(localFileName))
            {
                blockBlob.UploadFromStream(fileStream);
                return blockBlob.SnapshotQualifiedUri.ToString();
            }
        }
    }
}