namespace FileSystemProject.Models.ResponseModels
{
    public class FolderContentResponseModel
    {
        public string Name { get; set; }
        public DateTime ModifiedDate { get; set; }
        public string Type { get; set; }
        public long? Size { get; set; }
        public Uri FileDownloadUri { get; set; }
    }
}
