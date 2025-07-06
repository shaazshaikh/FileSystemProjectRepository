namespace FileSystemProject.Models.RequestModels
{
    public class StorePlaylistFileRequestModel
    {
        public string StreamingPath { get; set; }
        public string FileId { get; set; }
        public int? Duration { get; set; }
        public string Type { get; set; }
        public string Resolution { get; set; }
    }
}
