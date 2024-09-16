using FileSystemProject.Enums;

namespace FileSystemProject.Models.ResponseModels
{
    public class FolderResponseModel
    {
        public Guid? ParentFolderId { get; set; }
        public Guid Id { get; set; }
        public string FolderPath { get; set; }
        public string FolderName { get; set; }
        public long? FolderSize { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public FileOrFolderType FolderType { get; set; } = FileOrFolderType.Folder;
    }
}
