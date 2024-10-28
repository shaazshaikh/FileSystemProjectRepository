using Azure.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Specialized;
using FileSystemProject.Models.ResponseModels;
using Microsoft.AspNetCore.Mvc;

namespace FileSystemProject.Repository
{
    public interface IFileBlobRepository
    {
        Task<List<Uri>> UploadFileChunks(string userId, [FromForm] IFormFile file, string folderPath, string parentFolderId, int chunkIndex, int totalNumberOfChunks, string fileBlobId,  string fileName, string fileExtension, int totalFileSize);
        Task<List<FileResponseModel>> GetBlobFiles(string userId, string parentFolderId);
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

        //public async Task MergeAllChunks(string userId, string fileBlobId, string folderPath, string parentFolderId, string fileExtension, int totalNumberOfChunks, int totalFileSize, string chunkName)
        //{
        //    var finalBlobName = $"{userId}/{folderPath}/{fileBlobId}{fileExtension}";
        //    var blobContainer = _blobServiceClient.GetBlobContainerClient(Configuration["FileStorage:FileContainerName"]);
        //    var finalBlob = blobContainer.GetBlobClient(finalBlobName);
        //    var filePath = $"{folderPath}/{chunkName}";

        //    //start merging file chunks
        //    using (var outputStream = new MemoryStream())
        //    {
        //        for(int chunkIndex = 0; chunkIndex < totalNumberOfChunks; chunkIndex++)
        //        {
        //            var chunkBlobName = $"{userId}/{folderPath}/{fileBlobId}{fileExtension}.part{chunkIndex}";
        //            var chunkBlob = blobContainer.GetBlobClient(chunkBlobName);

        //            //download each chunk
        //            var downloadStream = await chunkBlob.OpenReadAsync();
        //            await downloadStream.CopyToAsync(outputStream);

        //            //delete chunk after appending
        //            await chunkBlob.DeleteIfExistsAsync();

        //        }

        //        // upload the merged file
        //        outputStream.Position = 0;
        //        await finalBlob.UploadAsync(outputStream, true);
        //    }

        //    //add file entry in tble after entore file is uploaded
        //    Guid? parentFolderGuid = parentFolderId != null ? Guid.Parse(parentFolderId) : null;
        //    await _fileRepository.InsertFileEntry(userId, $"{fileBlobId}{fileExtension}", totalFileSize, finalBlob.Uri.ToString(), parentFolderGuid, filePath, false, false, DateTime.UtcNow, DateTime.UtcNow);
        //}

        public async Task<List<FileResponseModel>> GetBlobFiles(string userId, string parentFolderId)
        {
            var filesFromDatabase = await _fileRepository.GetFiles(userId, parentFolderId);
            var blobUris = new List<FileResponseModel>();
            var blobContainer = _blobServiceClient.GetBlobContainerClient(Configuration["FileStorage:FileContainerName"]);
            //var blobsPath = $"{userId}/{folderPath}";
            //await foreach (var item in blobContainer.GetBlobsAsync(prefix: blobsPath))
            //{
            //    var uri = blobContainer.GetBlobClient(item.Name).Uri;
            //    blobUris.Add(uri);
            //}

            foreach(var file in filesFromDatabase)
            {
                //var blobName = new Uri(file.StoragePath).AbsolutePath.Replace("/filesystemcontainer/","").TrimStart('/');
                //var blobClient = blobContainer.GetBlobClient(blobName);
                //if(await blobClient.ExistsAsync())
                //{
                //    var fileUri = blobClient.Uri;
                //    blobUris.Add(new FileResponseModel
                //    {
                //        FileName = file.FileName,
                //        FilePath = file.FilePath,
                //        FileSize = file.FileSize,
                //        FileDownloadUri = fileUri,
                //        ModifiedDate = file.ModifiedDate,
                //        StoragePath = file.StoragePath
                //    });
                //}

                var fileUri = new Uri(file.StoragePath);
                blobUris.Add(new FileResponseModel
                {
                    FileName = file.FileName,
                    FilePath = file.FilePath,
                    FileSize = file.FileSize,
                    FileDownloadUri = fileUri,
                    ModifiedDate = file.ModifiedDate,
                    StoragePath = file.StoragePath
                });
            }

            return blobUris;
        }
    }
}
