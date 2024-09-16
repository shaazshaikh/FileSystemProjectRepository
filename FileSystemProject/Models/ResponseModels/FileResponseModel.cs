using FileSystemProject.Enums;

namespace FileSystemProject.Models.ResponseModels
{
    public class FileResponseModel
    {
        public string FileName { get; set; }
        public long FileSize { get; set; }
        public string StoragePath { get; set; }
        public string FilePath { get; set; }
        public DateTime ModifiedDate { get; set; }
        public Uri FileDownloadUri { get; set; }
        public FileOrFolderType FileType { get; set; } = FileOrFolderType.File;
    }
}
