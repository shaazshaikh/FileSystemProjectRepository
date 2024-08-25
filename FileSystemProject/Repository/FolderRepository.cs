using Azure.Storage.Blobs;
using Dapper;
using System.Data;

namespace FileSystemProject.Repository
{
    public interface IFolderRepository
    {
        Task<bool> InsertFolderEntry(string userId, string parentFolderId, string folderPath);
        Task<bool> DeleteFolderEntry();
    }

    public class FolderRepository : IFolderRepository
    {
        private readonly IDbConnection _dbConnection;

        public FolderRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task<bool> InsertFolderEntry(string userId, string parentFolderId, string folderPath)
        {
            string folderName = folderPath.Split('/').Last();
            Guid? parentFolderGuid = parentFolderId != null ? Guid.Parse(parentFolderId) : null;
            string insertFolderQuery = "insert into Folders(Id, UserId, FolderName, FolderSize, ParentFolderId, FolderPath, IsDeleted, IsDeletedByUser, CreatedDate, ModifiedDate, StoragePath) values(NewId(), @UserId, @FolderName, null, @ParentFolderId, @FolderPath, 0, 0, getDate(), getDate(), null)";

            int rowsAffected = await _dbConnection.ExecuteAsync(insertFolderQuery, new { UserId = userId, FolderName = folderName, ParentFolderId = parentFolderGuid, FolderPath = folderPath });
            if(rowsAffected > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task<bool> DeleteFolderEntry()
        {
            return true;
        }
    }
}
