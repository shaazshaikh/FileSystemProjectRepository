using FileSystemProject.Models.RequestModels;
using FileSystemProject.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
//using Swashbuckle.AspNetCore.SwaggerUI; //uncomment to use swagger


// The WebApi folder and Controller folder were created by me
namespace FileSystemProject.WebApi.Controller
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class FileController : ControllerBase
    {
        private readonly IFileBlobRepository _fileBlobRepository;

        public FileController(IFileBlobRepository fileBlobRepository)
        {
            _fileBlobRepository = fileBlobRepository;
        }

        [HttpPost]
        [Route("uploadFiles", Name = "UploadFile")]
        public async Task<List<Uri>> UploadFiles([FromForm] IFormFile file, [FromForm] string folderPath, [FromForm] string parentFolderId)
        {
            var userId = HttpContext.Items["UserId"]?.ToString();
            var blobUris = await _fileBlobRepository.UploadFilesAsync(userId,file, folderPath, parentFolderId);

            return blobUris;
        }

        [HttpPost]
        [Route("getFiles", Name = "GetFile")]
        public async Task<List<Uri>> GetFiles([FromBody] FileRequestModel obj)
        {
            var userId = HttpContext.Items["UserId"]?.ToString();
            var blobUris = await _fileBlobRepository.GetFilesAsync(userId, obj.FolderPath);

            return blobUris;
        }
    }
}
