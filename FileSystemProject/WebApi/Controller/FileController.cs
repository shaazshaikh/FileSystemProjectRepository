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
        private readonly IFileRepository _fileRepository;

        public FileController(IFileBlobRepository fileBlobRepository, IFileRepository fileRepository)
        {
            _fileBlobRepository = fileBlobRepository;
            _fileRepository = fileRepository;
        }

        [HttpPost]
        [Route("uploadFilesInChunks", Name = "UploadFilesInChunks")]
        public async Task<List<Uri>> UploadFilesInChunks([FromForm] IFormFile fileChunk, [FromForm] string folderPath, [FromForm] string parentFolderId, [FromForm] int chunkIndex, [FromForm] int totalNumberOfChunks, [FromForm] string fileBlobId, [FromForm] string fileName, [FromForm] string fileExtension, [FromForm] int totalFileSize = 0)
        {
            var userId = HttpContext.Items["UserId"]?.ToString();
            var blobUris = await _fileBlobRepository.UploadFileChunks(userId, fileChunk, folderPath, parentFolderId, chunkIndex, totalNumberOfChunks, fileBlobId, fileName, fileExtension, totalFileSize);

            return blobUris;
        }

        [HttpPost]
        [Route("StorePlaylistFile", Name = "StorePlaylistFile")]
        public async Task<IActionResult> StorePlaylistFile(StorePlaylistFileRequestModel model)
        {
            var isEntryAdded = await _fileRepository.InsertVideoEntry(model);
            return Ok(isEntryAdded);
        }

        //[HttpGet]
        //[Route("getStreamingPath/{fileId}", Name = "GetStreamingPath")]
        //public async Task<IActionResult> GetStreamingPath(string fileId)
        //{
        //    var playlistFileUrl = await _fileRepository.GetStreamingPathUrl(fileId);
        //    return Ok(playlistFileUrl);
        //}

        [HttpGet]
        [Route("getSASUrl", Name = "GetSASUrl")]
        public IActionResult GetSASUrl([FromQuery] string filePath)
        {
            var sasUrl =  _fileBlobRepository.GenerateSASUrlForFile(filePath);
            return Ok(sasUrl);
        }
    }
}
