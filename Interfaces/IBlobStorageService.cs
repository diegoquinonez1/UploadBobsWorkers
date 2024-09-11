using UploadBobsWorkers.Models;

namespace UploadBobsWorkers.Interfaces
{
    public interface IBlobStorageService
    {
        public Task<string?> UploadFileToBlobAsync(string strFileName, string contecntType, string ContentData);
        public Task<string?> DownloadFileToBlobAsync(string strFileName);
        public Task<List<BlobItemDTO>?> GetAllBlobs();
    }
}