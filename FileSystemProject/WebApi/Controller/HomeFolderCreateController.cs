using FileSystemProject.Models;
using FileSystemProject.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FileSystemProject.WebApi.Controller
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class HomeFolderCreateController : ControllerBase
    {
        private readonly IFolderRepository _folderRepository;
        public HomeFolderCreateController(IFolderRepository folderRepository)
        {
            _folderRepository = folderRepository;
        }

        [HttpPost]
        [Route("createHomeFolder", Name = "CreateHomeFolder")]
        public async Task<IActionResult> CreateHomeFolder([FromBody] CreateFolderModel createFolderModel)
        {
            var userId = HttpContext.Items["UserId"]?.ToString();
            var isUploaded = await _folderRepository.InsertFolderEntry(userId, null, createFolderModel.FolderPath);
            return Ok(isUploaded);
        }
    }
}
