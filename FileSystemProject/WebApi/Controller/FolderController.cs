using FileSystemProject.Models.RequestModels;
using FileSystemProject.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FileSystemProject.WebApi.Controller
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class FolderController : ControllerBase
    {
        private readonly IFolderRepository _folderRepository;
        public FolderController(IFolderRepository folderRepository)
        {
            _folderRepository = folderRepository;
        }

        [HttpPost]
        [Route("createFolder", Name = "CreateFolder")]
        public async Task<IActionResult> CreateFolder([FromBody] FolderRequestModel createFolderModel)
        {
            var userId = HttpContext.Items["UserId"]?.ToString();
            var isUploaded = await _folderRepository.InsertFolderEntry(userId, createFolderModel.ParentFolderId, createFolderModel.FolderPath);
            return Ok(isUploaded);
        }

        [HttpGet]
        [Route("getHomeFolderDetails/{folderName}", Name = "GetHomeFolderDetails")]
        public async Task<IActionResult> GetHomeFolderDetails(string folderName)
        {
            var userId = HttpContext.Items["UserId"]?.ToString();
            var folderDetails = await _folderRepository.FetchHomeFolderDetails(userId, folderName);
            return Ok(folderDetails);
        }

        [HttpGet]
        [Route("getFolderDetails/{folderId}", Name = "GetFolderDetails")]
        public async Task<IActionResult> GetFolderDetails(string folderId)
        {
            var userId = HttpContext.Items["UserId"]?.ToString();
            var folderDetails = await _folderRepository.FetchFolderDetails(userId, folderId);
            return Ok(folderDetails);
        }
    }
}
