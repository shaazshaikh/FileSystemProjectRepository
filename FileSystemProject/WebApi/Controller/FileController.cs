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
        [Route("uploadFilesInChunks", Name = "UploadFilesInChunks")]
        public async Task<List<Uri>> UploadFilesInChunks([FromForm] IFormFile fileChunk, [FromForm] string folderPath, [FromForm] string parentFolderId, [FromForm] int chunkIndex, [FromForm] int totalNumberOfChunks, [FromForm] string fileBlobId, [FromForm] string fileName, [FromForm] string fileExtension, [FromForm] int totalFileSize = 0)
        {
            var userId = HttpContext.Items["UserId"]?.ToString();
            var blobUris = await _fileBlobRepository.UploadFileChunks(userId, fileChunk, folderPath, parentFolderId, chunkIndex, totalNumberOfChunks, fileBlobId, fileName, fileExtension, totalFileSize);

            return blobUris;
        }
    }
}
