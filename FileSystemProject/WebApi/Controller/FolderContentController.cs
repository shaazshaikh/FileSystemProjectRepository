using FileSystemProject.Models.ResponseModels;
using FileSystemProject.Repository;
using Microsoft.AspNetCore.Mvc;

namespace FileSystemProject.WebApi.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class FolderContentController : ControllerBase
    {
        private readonly IFileBlobRepository _fileBlobRepository;
        private readonly IFolderRepository _folderRepository;
        public FolderContentController(IFileBlobRepository fileBlobRepository, IFolderRepository folderRepository) 
        {
            _fileBlobRepository = fileBlobRepository;
            _folderRepository = folderRepository;
        }
        [HttpGet]
        [Route("getFolderContents/{parentFolderId}", Name = "GetFolderContents")]
        public async Task<List<FolderContentResponseModel>> GetFolderContents(string parentFolderId)
        {
            var userId = HttpContext.Items["UserId"]?.ToString();
            var filesTask =  _fileBlobRepository.GetBlobFiles(userId, parentFolderId);
            var foldersTask =  _folderRepository.FetchFolderContents(userId, parentFolderId);

            await Task.WhenAll(filesTask, foldersTask);

            var files = (await filesTask).Select(file => new FolderContentResponseModel
            {
                Name = file.FileName,
                ModifiedDate = file.ModifiedDate,
                Type = file.FileType.ToString(),
                Size = file.FileSize,
                FileDownloadUri = file.FileDownloadUri
            }).ToList();

            var folders = (await foldersTask).Select(folder => new FolderContentResponseModel
            {
                Name = folder.FolderName,
                ModifiedDate = folder.ModifiedDate,
                Type = folder.FolderType.ToString(),
                Size = null,
            }).ToList();

            return files.Concat(folders).ToList();
        }
    }
}
