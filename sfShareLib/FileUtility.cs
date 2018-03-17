using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Azure; // Namespace for CloudConfigurationManager
using Microsoft.WindowsAzure.Storage; // Namespace for CloudStorageAccount
using Microsoft.WindowsAzure.Storage.Blob; // Namespace for Blob storage types
using System.IO;

namespace sfShareLib
{
    public class FileUtility
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
            // Create or overwrite the "blockBlob" blob with contents from a local file.
            using (var fileStream = File.OpenRead(localFileName))
            {
                blockBlob.UploadFromStream(fileStream);
                return blockBlob.SnapshotQualifiedUri.ToString();
            }
        }

        public bool CheckImageExtensionName(string columnName, string extensionName)
        {
            extensionName = LowerAndFilterString(extensionName);
            columnName = LowerAndFilterString(columnName);
            if (columnName.Equals("image") && (extensionName.Equals("png") || extensionName.Equals("jpg")))
                return true;

            return false;
        }

        public string LowerAndFilterString(string str)
        {
            char[] trimChar = { '\"' };
            return str.ToLower().Trim(trimChar); ;
        }
    }
}
