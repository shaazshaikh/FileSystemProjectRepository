using Azure.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Specialized;
using Azure.Storage.Sas;
using FileSystemProject.Models.ResponseModels;
using Microsoft.AspNetCore.Mvc;

namespace FileSystemProject.Repository
{
    public interface IFileBlobRepository
    {
        Task<List<Uri>> UploadFileChunks(string userId, [FromForm] IFormFile file, string folderPath, string parentFolderId, int chunkIndex, int totalNumberOfChunks, string fileBlobId,  string fileName, string fileExtension, int totalFileSize);
        Task<List<FileResponseModel>> GetBlobFiles(string userId, string parentFolderId);
        Uri? GenerateSASUrlForFile(string filePath);
    }
    public class FileBlobRepository : IFileBlobRepository
    {
        private readonly BlobServiceClient _blobServiceClient;
        private readonly IFileRepository _fileRepository;
        public IConfiguration Configuration { get; }
        public FileBlobRepository(IConfiguration configuration, IFileRepository fileRepository) 
        {
            Configuration = configuration;
            var credential = new StorageSharedKeyCredential(Configuration["FileStorage:StorageAccountName"], Configuration["FileStorage:StorageAccountAccessKey"]);
            var blobUri = $"https://{Configuration["FileStorage:StorageAccountName"]}.blob.core.windows.net";
            _blobServiceClient = new BlobServiceClient(new Uri(blobUri), credential);
            _fileRepository = fileRepository;
        }

        public async Task<List<Uri>> UploadFileChunks(string userId, [FromForm] IFormFile fileChunk, string folderPath, string parentFolderId, int chunkIndex, int totalNumberOfChunks, string fileBlobId, string fileName, string fileExtension, int totalFileSize)
        {
            var blobUris = new List<Uri>();
            //var fileBlobId = Guid.NewGuid();
            var blobName = $"{userId}/{folderPath}/{fileBlobId}{fileExtension}";
            var filePath = $"{folderPath}/{fileName}";

            var blobContainer = _blobServiceClient.GetBlobContainerClient(Configuration["FileStorage:FileContainerName"]);
            var blob = blobContainer.GetAppendBlobClient(blobName);

            if(chunkIndex == 0 && !await blob.ExistsAsync())
            {
                await blob.CreateAsync();
            }

            using (var stream = fileChunk.OpenReadStream())
            {
                await blob.AppendBlockAsync(stream);
            }
            blobUris.Add(blob.Uri);

            if (chunkIndex + 1 == totalNumberOfChunks)
            {
                Guid? parentFolderGuid = parentFolderId != null ? Guid.Parse(parentFolderId) : null;
                await _fileRepository.InsertFileEntry(userId, fileName, totalFileSize, blob.Uri.ToString(), parentFolderGuid, filePath, false, false, DateTime.UtcNow, DateTime.UtcNow);
            }

            return blobUris;
        }

        public async Task<List<FileResponseModel>> GetBlobFiles(string userId, string parentFolderId)
        {
            var filesFromDatabase = await _fileRepository.GetFiles(userId, parentFolderId);
            var blobUris = new List<FileResponseModel>();
            var blobContainer = _blobServiceClient.GetBlobContainerClient(Configuration["FileStorage:FileContainerName"]);

            foreach(var file in filesFromDatabase)
            {
                var fileUri = new Uri(file.StoragePath);
                blobUris.Add(new FileResponseModel
                {
                    Id = file.Id,
                    FileName = file.FileName,
                    FilePath = file.FilePath,
                    FileSize = file.FileSize,
                    FileDownloadUri = fileUri,
                    ModifiedDate = file.ModifiedDate,
                    StoragePath = file.StoragePath,
                    ParentFolderId = file.ParentFolderId
                });
            }

            return blobUris;
        }

        public Uri? GenerateSASUrlForFile(string filePath)
        {
            var blobContainer = _blobServiceClient.GetBlobContainerClient(Configuration["FileStorage:FileContainerName"]);
            var blob = blobContainer.GetBlobClient(filePath);

            var sas = new BlobSasBuilder
            {
                BlobContainerName = Configuration["FileStorage:FileContainerName"],
                BlobName = filePath,
                Resource = "b",
                ExpiresOn = DateTimeOffset.UtcNow.AddMinutes(30)
            };

            sas.SetPermissions(BlobAccountSasPermissions.Read);
            var sasUrl = blob.GenerateSasUri(sas);

            return sasUrl;

        }
    }
}
