using Dapper;
using FileSystemProject.Models.ResponseModels;
using System.Data;

namespace FileSystemProject.Repository
{
    public interface IFileRepository
    {
        Task<bool> InsertFileEntry(string userId, string fileName, long fileSize, string storagePath, Guid? parentFolderId, string filePath, bool isDeleted, bool isDeletedByUser, DateTime createdDate, DateTime modifiedDate);
        Task<bool> DeleteFileEntry();
        Task<IEnumerable<FileResponseModel>> GetFiles(string userId, string parentFolderId);
    }

    public class FileRepository : IFileRepository
    {
        private readonly IDbConnection _dbConnection;
        public FileRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }
        public async Task<bool> InsertFileEntry(string userId, string fileName, long fileSize, string storagePath, Guid? parentFolderId, string filePath, bool isDeleted, bool isDeletedByUser, DateTime createdDate, DateTime modifiedDate)
        {
            string insertFileQuery = "insert into Files(Id, UserId, FileName, FileSize, StoragePath, ParentFolderId, FilePath, IsDeleted, IsDeletedByUser, CreatedDate, ModifiedDate) values(NewId(), @UserId, @FileName, @FileSize, @StoragePath, @ParentFolderId, @FilePath, 0, 0, getDate(), getDate())";

            int rowsAffected = await _dbConnection.ExecuteAsync(insertFileQuery, new { UserId = userId, FileName = fileName, FileSize = fileSize, StoragePath = storagePath, ParentFolderId = parentFolderId, FilePath = filePath });
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
            string getFilesQuery = "select fileName, fileSize, modifiedDate, storagePath, filePath from Files where userId = @UserId and parentFolderId = @ParentFolderId";
            Guid? parentFolderGuid = parentFolderId != null ? Guid.Parse(parentFolderId) : null;

            var files = await _dbConnection.QueryAsync<FileResponseModel>(getFilesQuery, new { userId = Guid.Parse(userId), parentFolderId = parentFolderGuid });

            return files;
        }
        
    }
}
