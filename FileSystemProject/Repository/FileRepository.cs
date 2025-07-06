using Dapper;
using FileSystemProject.Models.RequestModels;
using FileSystemProject.Models.ResponseModels;
using System.Data;

namespace FileSystemProject.Repository
{
    public interface IFileRepository
    {
        Task<Guid?> InsertFileEntry(string userId, string fileName, long fileSize, string storagePath, Guid? parentFolderId, string filePath, bool isDeleted, bool isDeletedByUser, DateTime createdDate, DateTime modifiedDate);
        Task<bool> InsertVideoEntry(StorePlaylistFileRequestModel model);
        Task<bool> DeleteFileEntry();
        Task<IEnumerable<FileResponseModel>> GetFiles(string userId, string parentFolderId);
        //Task<string?> GetStreamingPathUrl(string fileId);
    }

    public class FileRepository : IFileRepository
    {
        private readonly IDbConnection _dbConnection;
        public FileRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }
        public async Task<Guid?> InsertFileEntry(string userId, string fileName, long fileSize, string storagePath, Guid? parentFolderId, string filePath, bool isDeleted, bool isDeletedByUser, DateTime createdDate, DateTime modifiedDate)
        {
            var newFileId = Guid.NewGuid();
            string insertFileQuery = "insert into Files(Id, UserId, FileName, FileSize, StoragePath, ParentFolderId, FilePath, IsDeleted, IsDeletedByUser, CreatedDate, ModifiedDate) values(@Id, @UserId, @FileName, @FileSize, @StoragePath, @ParentFolderId, @FilePath, 0, 0, getDate(), getDate())";

            int rowsAffected = await _dbConnection.ExecuteAsync(insertFileQuery, new { Id = newFileId, UserId = userId, FileName = fileName, FileSize = fileSize, StoragePath = storagePath, ParentFolderId = parentFolderId, FilePath = filePath });
            if (rowsAffected > 0)
            {
                return newFileId;
            }
            else
            {
                return (Guid?)null;
            }
        }

        public async Task<bool> InsertVideoEntry(StorePlaylistFileRequestModel model)
        {
            string insertVideoQuery = "insert into Videos(Id, FileId, Duration, Type, Resolution, StreamingPath, CreatedDate, ModifiedDate, IsSystemGenerated) values(NewId(), @FileId, null, 'mp4', @Resolution, @StreamingPath, getDate(), getDate(), 1)";

            int rowsAffected = await _dbConnection.ExecuteAsync(insertVideoQuery, new { FileId = model.FileId, Resolution = model.Resolution, StreamingPath = model.StreamingPath });
            if (rowsAffected > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task<bool> DeleteFileEntry()
        {
            return true;
        }

        public async Task<IEnumerable<FileResponseModel>> GetFiles(string userId, string parentFolderId)
        {
            string getFilesQuery = "select Id, parentFolderId, fileName, fileSize, modifiedDate, storagePath, filePath from Files where userId = @UserId and parentFolderId = @ParentFolderId";
            Guid? parentFolderGuid = parentFolderId != null ? Guid.Parse(parentFolderId) : null;

            var files = await _dbConnection.QueryAsync<FileResponseModel>(getFilesQuery, new { userId = Guid.Parse(userId), parentFolderId = parentFolderGuid });

            return files;
        }

        //public async Task<string?> GetStreamingPathUrl(string fileId)
        //{
        //    string getStreamingPathQuery = "select StreamingPath from Videos where fileId = @FileId and Resolution = '720p'";
        //    Guid fileGuid = Guid.Parse(fileId);
        //    var streamingPath = await _dbConnection.QueryFirstOrDefaultAsync<string>(getStreamingPathQuery, new { FileId = fileGuid });
        //    return streamingPath;
        //}
    }
}
