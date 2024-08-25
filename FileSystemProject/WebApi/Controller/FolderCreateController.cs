using FileSystemProject.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FileSystemProject.WebApi.Controller
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class FolderCreateController : ControllerBase
    {
        private readonly IFolderRepository _folderRepository;
        public FolderCreateController(IFolderRepository folderRepository)
        {
            _folderRepository = folderRepository;
        }

        [HttpPost]
        [Route("createFolder", Name = "CreateFolder")]
        public async Task<IActionResult> CreateFolder(string parentFolderId, string folderPath)
        {
            var userId = HttpContext.Items["UserId"]?.ToString();
            var isUploaded = await _folderRepository.InsertFolderEntry(userId, parentFolderId, folderPath);
            return Ok(isUploaded);
        }
    }
}
