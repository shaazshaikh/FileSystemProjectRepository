using Azure.Storage;
using Azure.Storage.Blobs;

namespace FileSystemProject.Repository
{
    public interface IFolderBlobRepository
    {
        Task<List<Uri>> CreateFolderAsync(string userId,string folderPath);
    }

    public class FolderBlobRepository : IFolderBlobRepository
    {
        private readonly BlobServiceClient _blobServiceClient;
        public IConfiguration Configuration { get; }

        public FolderBlobRepository(IConfiguration configuration)
        {
            //_blobServiceClient =;
            Configuration = configuration;
            var credential = new StorageSharedKeyCredential(Configuration["FileStorage:StorageAccountName"], Configuration["FileStorage:StorageAccountAccessKey"]);
            var blobUri = $"https://{Configuration["FileStorage:StorageAccountName"]}.blob.core.windows.net";
            _blobServiceClient = new BlobServiceClient(new Uri(blobUri), credential);
        }

        public async Task<List<Uri>> CreateFolderAsync(string userId, string folderPath)
        {
            var blobUris = new List<Uri>();
            var blobName = $"{userId}/{folderPath}";
            var blobContainer = _blobServiceClient.GetBlobContainerClient(Configuration["FileStorage:FileContainerName"]);
            var blob = blobContainer.GetBlobClient(blobName);

            using(var stream = new MemoryStream())
            {
                await blob.UploadAsync(stream);
            }
            return blobUris;
        }
    }
}
