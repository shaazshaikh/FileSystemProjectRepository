using Azure.Storage;
using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Mvc;
//using Swashbuckle.AspNetCore.SwaggerUI; //uncomment to use swagger


// The WebApi folder and Controller folder were created by me
namespace FileSystemProject.WebApi.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class FileUploadController : ControllerBase
    {
        private readonly string _storageAccountName = ""; // Storage Account Name
        private readonly string _fileContainerName = ""; // Storage Account Name
        private readonly string _storageAccountAccessKey = ""; // Storage Account Access Key
        private readonly BlobServiceClient _blobServiceClient;

        public FileUploadController()
        {
            var credential = new StorageSharedKeyCredential(_storageAccountName, _storageAccountAccessKey);
            var blobUri = $"https://{_storageAccountName}.blob.core.windows.net";
            _blobServiceClient = new BlobServiceClient(new Uri(blobUri), credential);
        }

        //[HttpPost]
        //[Route("uploadFiles/{name}",Name = "UploadFile")]
        //public async Task<List<Uri>> UploadFilesAsync([FromForm] IFormFile file, string name)
        //{
        //    var _name = name;
        //    var blobUris = new List<Uri>();
        //    //string filePath = "Hi.txt";
        //    var blobName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
        //    var blobContainer = _blobServiceClient.GetBlobContainerClient(_fileContainerName);
        //    var blob = blobContainer.GetBlobClient($"HomeFolder/{blobName}");

        //    using( var stream = file.OpenReadStream())
        //    {
        //        await blob.UploadAsync(stream, true);
        //    }
        //    //await blob.UploadAsync(filePath, true);
        //    blobUris.Add(blob.Uri);

        //    return blobUris;
        //}

        [HttpPost]
        [Route("uploadFiles", Name = "UploadFile")]
        public async Task<List<Uri>> UploadFilesAsync([FromForm] IFormFile file)
        {
            //var _name = name;
            var blobUris = new List<Uri>();
            //string filePath = "Hi.txt";
            var blobName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            var blobContainer = _blobServiceClient.GetBlobContainerClient(_fileContainerName);
            var blob = blobContainer.GetBlobClient($"HomeFolder/{blobName}");

            using (var stream = file.OpenReadStream())
            {
                await blob.UploadAsync(stream, true);
            }
            //await blob.UploadAsync(filePath, true);
            blobUris.Add(blob.Uri);

            return blobUris;
        }


    }
}
