using Azure.Storage.Blobs;
using Dapper;
using FileSystemProject.Models.ResponseModels;
using System.Data;

namespace FileSystemProject.Repository
{
    public interface IFolderRepository
    {
        Task<bool> InsertFolderEntry(string userId, string parentFolderId, string folderPath);
        Task<bool> DeleteFolderEntry();
        Task<FolderResponseModel> FetchHomeFolderDetails(string userId, string folderName);
        Task<FolderResponseModel> FetchFolderDetails(string userId, string folderId);
        Task<IEnumerable<FolderResponseModel>> FetchFolderContents(string userId, string parentFolderId);
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

        public async Task<FolderResponseModel> FetchHomeFolderDetails(string userId, string folderName)
        {
            string sqlQuery = "select Id, ParentFolderId, FolderPath, FolderName, CreatedDate, ModifiedDate from folders where userId = @userId and folderName = @folderName and parentFolderId is null";
            FolderResponseModel homeFolderDetails = await _dbConnection.QueryFirstOrDefaultAsync<FolderResponseModel>(sqlQuery, new
            {
                userId = Guid.Parse(userId),
                foldername = folderName
            });

            return homeFolderDetails;
        }

        public async Task<FolderResponseModel> FetchFolderDetails(string userId, string folderId)
        {
            string sqlQuery = "select Id, ParentFolderId, FolderPath, FolderName, CreatedDate, ModifiedDate from folders where userId = @userId and Id = @folderId";
            FolderResponseModel folderDetails = await _dbConnection.QueryFirstOrDefaultAsync<FolderResponseModel>(sqlQuery, new
            {
                userId = Guid.Parse(userId),
                folderId = Guid.Parse(folderId)
            });

            return folderDetails;
        }

        public async Task<IEnumerable<FolderResponseModel>> FetchFolderContents(string userId, string parentFolderId)
        {

            string sqlQuery = "select Id, ParentFolderId, FolderPath, FolderName, CreatedDate, ModifiedDate from folders where userId = @userId and parentFolderId = @parentFolderId";
            var folderContents = await _dbConnection.QueryAsync<FolderResponseModel>(sqlQuery, new
            {
                userId = Guid.Parse(userId),
                parentFolderId = Guid.Parse(parentFolderId)
            });

            return folderContents;
        }

        public async Task<bool> DeleteFolderEntry()
        {
            return true;
        }
    }
}
