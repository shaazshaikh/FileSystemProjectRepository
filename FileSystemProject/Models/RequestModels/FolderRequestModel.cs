namespace FileSystemProject.Models.RequestModels
{
    public class FolderRequestModel
    {
        public string? ParentFolderId { get; set; }
        public string FolderPath { get; set; }
        public string FolderName { get; set; }

    }
}
