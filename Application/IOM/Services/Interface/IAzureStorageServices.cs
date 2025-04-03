using System.IO;
using Microsoft.Azure.Storage.Blob;

namespace IOM.Services.Interface
{
    public interface IAzureStorageServices
    {
        CloudBlobClient AzureBlobClient { get; }
        string StoreProfileImage(Stream stream, string format);
    }
}