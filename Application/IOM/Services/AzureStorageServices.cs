using IOM.Utilities;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;
using System;
using System.IO;
using IOM.Services.Interface;

namespace IOM.Services
{
    public class AzureStorageServices : IAzureStorageServices
    {
        private CloudBlobClient _cloudBlobClient;

        public CloudBlobClient AzureBlobClient
        {
            get
            {
                if (_cloudBlobClient == null)
                {
                    var storageAccount = CloudStorageAccount.Parse(connectionString: AzureAppSettings.AzureConnectionString);

                    _cloudBlobClient = storageAccount?.CreateCloudBlobClient();
                }
                return _cloudBlobClient;
            }
        }

        public string StoreProfileImage(Stream stream, string format)
        {
            var filename = $"{Guid.NewGuid().ToString()}.{format}";

            var blobContainer = AzureBlobClient
              ?.GetContainerReference(containerName: AzureAppSettings.AzureDefaultContainer);

            blobContainer.CreateIfNotExists();
            blobContainer.SetPermissions(new BlobContainerPermissions
            {
                PublicAccess = BlobContainerPublicAccessType.Blob
            });

            var blockBlob = blobContainer
                ?.GetBlockBlobReference(blobName: filename);

            blockBlob.Properties.ContentType = "image";
            blockBlob?.UploadFromStream(source: stream);

            stream.Dispose();

            return blockBlob.Uri.AbsoluteUri;
        }
    }
}