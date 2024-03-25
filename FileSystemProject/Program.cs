using FileSystemProject;

var service = new FileStorageService();
await service.ListAllBlobObjectContainersAsync();
await service.UploadFilesAsync();
await service.AccessFlatBlobObjectAsync();
await service.DeleteBlobObjectAsync();


Console.ReadLine();