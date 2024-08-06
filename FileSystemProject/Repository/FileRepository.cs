using Azure.Storage;
using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Mvc;

namespace FileSystemProject.Repository
{
    public interface IFileRepository
    {
        Task<List<Uri>> UploadFilesAsync([FromForm] IFormFile file, string folderPath);
        Task<List<Uri>> GetFilesAsync(string folderPath);
    }
    public class FileRepository : IFileRepository
    {
        private readonly BlobServiceClient _blobServiceClient;
        public IConfiguration Configuration { get; }
        public FileRepository(IConfiguration configuration) 
        {
            Configuration = configuration;
            var credential = new StorageSharedKeyCredential(Configuration["FileStorage:StorageAccountName"], Configuration["FileStorage:StorageAccountAccessKey"]);
            var blobUri = $"https://{Configuration["FileStorage:StorageAccountName"]}.blob.core.windows.net";
            _blobServiceClient = new BlobServiceClient(new Uri(blobUri), credential);
        }

        public async Task<List<Uri>> UploadFilesAsync([FromForm] IFormFile file, string folderPath)
        {
            var blobUris = new List<Uri>();
            //var blobName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            var blobName = $"{folderPath}/{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            var blobContainer = _blobServiceClient.GetBlobContainerClient(Configuration["FileStorage:FileContainerName"]);
            //var blob = blobContainer.GetBlobClient($"HomeFolder/{blobName}");
            var blob = blobContainer.GetBlobClient(blobName);

            using (var stream = file.OpenReadStream())
            {
                await blob.UploadAsync(stream, true);
            }
            //await blob.UploadAsync(filePath, true);
            blobUris.Add(blob.Uri);

            return blobUris;
        }

        public async Task<List<Uri>> GetFilesAsync(string folderPath)
        {
            var blobUris = new List<Uri>();
            var blobContainer = _blobServiceClient.GetBlobContainerClient(Configuration["FileStorage:FileContainerName"]);

            await foreach (var item in blobContainer.GetBlobsAsync(prefix:folderPath))
            {
                var uri = blobContainer.GetBlobClient(item.Name).Uri;
                blobUris.Add(uri);
            }

            return blobUris;
        }
    }
}
