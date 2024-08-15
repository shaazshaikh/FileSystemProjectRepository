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
    public class FileUploadController : ControllerBase
    {
        private readonly IFileRepository _fileRepository;

        public FileUploadController(IFileRepository fileRepository)
        {
            _fileRepository = fileRepository;
        }

        [HttpPost]
        [Route("uploadFiles", Name = "UploadFile")]
        public async Task<List<Uri>> UploadFiles([FromForm] IFormFile file, [FromForm] string folder)
        {
            var userId = HttpContext.Items["UserId"]?.ToString();
            var blobUris = await _fileRepository.UploadFilesAsync(file, folder);

            return blobUris;
        }

        [HttpGet]
        [Route("getFiles", Name = "GetFile")]
        public async Task<List<Uri>> GetFiles()
        {
            var userId = HttpContext.Items["UserId"]?.ToString();
            var blobUris = await _fileRepository.GetFilesAsync(string.Empty);

            return blobUris;
        }
    }
}
