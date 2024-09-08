﻿using Azure.Storage;
using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Mvc;

namespace FileSystemProject.Repository
{
    public interface IFileBlobRepository
    {
        Task<List<Uri>> UploadFilesAsync(string userId, [FromForm] IFormFile file, string folderPath, string parentFolderId);
        Task<List<Uri>> GetFilesAsync(string userId, string folderPath);
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

        public async Task<List<Uri>> UploadFilesAsync(string userId, [FromForm] IFormFile file, string folderPath, string parentFolderId)
        {
            var fileExtension = Path.GetExtension(file.FileName);
            var filePath = $"{folderPath}/{file.FileName}";
            var blobUris = new List<Uri>();
            var fileBlobId = Guid.NewGuid();
            var blobName = $"{userId}/{folderPath}/{fileBlobId}{fileExtension}";
            var blobContainer = _blobServiceClient.GetBlobContainerClient(Configuration["FileStorage:FileContainerName"]);
            var blob = blobContainer.GetBlobClient(blobName);

            using (var stream = file.OpenReadStream())
            {
                await blob.UploadAsync(stream, true);
            }
            blobUris.Add(blob.Uri);
            Guid? parentFolderGuid = parentFolderId != null ? Guid.Parse(parentFolderId) : null;
            await  _fileRepository.InsertFileEntry(userId, file.FileName, file.Length, blob.Uri.ToString(), parentFolderGuid, filePath,false,false,DateTime.UtcNow, DateTime.UtcNow);

            return blobUris;
        }

        public async Task<List<Uri>> GetFilesAsync(string userId, string folderPath)
        {
            var blobUris = new List<Uri>();
            var blobContainer = _blobServiceClient.GetBlobContainerClient(Configuration["FileStorage:FileContainerName"]);
            var blobsPath = $"{userId}/{folderPath}";
            await foreach (var item in blobContainer.GetBlobsAsync(prefix: blobsPath))
            {
                var uri = blobContainer.GetBlobClient(item.Name).Uri;
                blobUris.Add(uri);
            }

            return blobUris;
        }
    }
}
