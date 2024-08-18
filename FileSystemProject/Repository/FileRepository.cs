using Dapper;
using System.Data;

namespace FileSystemProject.Repository
{
    public interface IFileRepository
    {
        Task InsertFileEntry(string UserId, string FileName, long FileSize, string StoragePath, Guid? ParentFolderId, string FilePath, bool IsDeleted, bool IsDeletedByUser, DateTime CreatedDate, DateTime ModifiedDate);
        Task<bool> DeleteFileEntry();
    }

    public class FileRepository : IFileRepository
    {
        private readonly IDbConnection _dbConnection;
        public FileRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }
        public async Task InsertFileEntry(string userId, string fileName, long fileSize, string storagePath, Guid? parentFolderId, string filePath, bool isDeleted, bool isDeletedByUser, DateTime createdDate, DateTime modifiedDate)
        {
            string insertFileQuery = "insert into Files(Id, UserId, FileName, FileSize, StoragePath, ParentFolderId, FilePath, IsDeleted, IsDeletedByUser, CreatedDate, ModifiedDate) values(NewId(), @UserId, @FileName, @FileSize, @StoragePath, null, @FilePath, 0, 0, getDate(), getDate())";

            await _dbConnection.ExecuteAsync(insertFileQuery, new { UserId = userId, FileName = fileName, FileSize = fileSize, StoragePath = storagePath, FilePath = filePath });
        }

        public async Task<bool> DeleteFileEntry()
        {
            return true;
        }
        
    }
}
