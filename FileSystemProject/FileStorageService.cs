using Azure.Storage;
using Azure.Storage.Blobs;

namespace FileSystemProject
{
    public class FileStorageService
    {
        private readonly string _storageAccountName = ""; // Storage Account Name
        private readonly string _fileContainerName = ""; // Storage Account Name
        private readonly string _storageAccountAccessKey = ""; // Storage Account Access Key
        private readonly BlobServiceClient _blobServiceClient;

        public FileStorageService()
        {
            var credential = new StorageSharedKeyCredential(_storageAccountName, _storageAccountAccessKey);
            var blobUri = $"https://{_storageAccountName}.blob.core.windows.net";
            _blobServiceClient = new BlobServiceClient(new Uri(blobUri), credential);
        }

        public async Task ListAllBlobObjectContainersAsync()
        {
            var containers = _blobServiceClient.GetBlobContainersAsync();
            await foreach (var container in containers)
            {
                Console.WriteLine(container.Name);
            }
        }

        public async Task<List<Uri>> UploadFilesAsync()
        {
            var blobUris = new List<Uri>();
            string filePath = "Hi.txt";
            var blobContainer = _blobServiceClient.GetBlobContainerClient(_fileContainerName);
            var blob = blobContainer.GetBlobClient($"HomeFolder/{filePath}");

            await blob.UploadAsync(filePath, true);
            blobUris.Add(blob.Uri);

            return blobUris;
        }

        public async Task AccessFlatBlobObjectAsync()
        {
            var blobContainer = _blobServiceClient.GetBlobContainerClient(_fileContainerName);
            var blobs = blobContainer.GetBlobsAsync();

            await foreach(var blob in blobs)
            {
                Console.WriteLine($"Blob files are {blob.Name}");
            }
        }

        public async Task DeleteBlobObjectAsync()
        {
            string fileName = "hello.Txt";
            var blobContainer = _blobServiceClient.GetBlobContainerClient(_fileContainerName);
            var blob = blobContainer.GetBlobClient($"today/{fileName}");

            await blob.DeleteIfExistsAsync();
        }
    }
}
