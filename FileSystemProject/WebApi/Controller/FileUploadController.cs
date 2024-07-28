﻿using Azure.Storage;
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
        private readonly BlobServiceClient _blobServiceClient;

        public IConfiguration Configuration { get; }

        public FileUploadController(IConfiguration configuration)
        {
            Configuration = configuration;
            var credential = new StorageSharedKeyCredential(Configuration["FileStorage:StorageAccountName"], Configuration["FileStorage:StorageAccountAccessKey"]);
            var blobUri = $"https://{Configuration["FileStorage:StorageAccountName"]}.blob.core.windows.net";
            _blobServiceClient = new BlobServiceClient(new Uri(blobUri), credential);
        }

        

        [HttpPost]
        //[Route("uploadFiles/{name}",Name = "UploadFile")]
        [Route("uploadFiles", Name = "UploadFile")]
        public async Task<List<Uri>> UploadFilesAsync([FromForm] IFormFile file)
        {
            //var _name = name;
            var blobUris = new List<Uri>();
            //string filePath = "Hi.txt";
            var blobName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            var blobContainer = _blobServiceClient.GetBlobContainerClient(Configuration["FileStorage:FileContainerName"]);
            var blob = blobContainer.GetBlobClient($"HomeFolder/{blobName}");

            using (var stream = file.OpenReadStream())
            {
                await blob.UploadAsync(stream, true);
            }
            //await blob.UploadAsync(filePath, true);
            blobUris.Add(blob.Uri);

            return blobUris;
        }

        [HttpGet]
        [Route("getFiles", Name = "GetFile")]
        public async Task<List<Uri>> GetFilesAsync()
        {
            var blobUris = new List<Uri>();
            var blobContainer = _blobServiceClient.GetBlobContainerClient(Configuration["FileStorage:FileContainerName"]);

            await foreach(var item in blobContainer.GetBlobsAsync())
            {
                var uri = blobContainer.GetBlobClient(item.Name).Uri;
                blobUris.Add(uri);
            }

            return blobUris;
        }


    }
}
