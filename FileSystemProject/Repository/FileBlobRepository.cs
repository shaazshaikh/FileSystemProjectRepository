using Azure.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Specialized;
using Azure.Storage.Queues;
using Azure.Storage.Sas;
using FileSystemProject.Models.ResponseModels;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;

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
                var fileId = await _fileRepository.InsertFileEntry(userId, fileName, totalFileSize, blob.Uri.ToString(), parentFolderGuid, filePath, false, false, DateTime.UtcNow, DateTime.UtcNow);

                if (IsItVideoFile(fileExtension))
                {
                    try
                    {
                        var queueClient = new QueueClient("UseDevelopmentStorage=true", "transcode-queue", new QueueClientOptions
                        {
                            MessageEncoding = QueueMessageEncoding.Base64
                        });

                        await queueClient.CreateIfNotExistsAsync();

                        var preMessage = new
                        {
                            UserId = userId,
                            FileId = fileId,
                            BlobUri = blob.Uri.ToString(),
                            FileName = fileName,
                            FileExtension = fileExtension,
                            BlobName = blobName,
                            FilePath = filePath
                        };

                        var jsonMessage = JsonConvert.SerializeObject(preMessage);
                        var utf8Bytes = Encoding.UTF8.GetBytes(jsonMessage);
                        var base64Message = Convert.ToBase64String(utf8Bytes);

                        if (queueClient.Exists())
                        {
                            await queueClient.SendMessageAsync(base64Message);
                        }
                        else
                        {
                            throw new Exception("queue not created yet");
                        }                      
                    }
                    catch(Exception e)
                    {
                        //handle exception
                    }
                    
                }
            }

            return blobUris;
        }

        private bool IsItVideoFile(string fileExtension)
        {
            var extensions = new List<string> { ".mp4", ".mov", ".avi", ".mkv" };
            return extensions.Contains(fileExtension.ToLower());
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

            //sas.SetPermissions(BlobAccountSasPermissions.Read);
            sas.SetPermissions(BlobSasPermissions.Read);
            var sasUrl = blob.GenerateSasUri(sas);

            return sasUrl;

        }
    }
}
