using System.Text;
using UploadBobsWorkers.Common;
using UploadBobsWorkers.Helpers;
using UploadBobsWorkers.Interfaces;
using UploadBobsWorkers.Models;

namespace UploadBobsWorkers.Services
{
    public class BlobStorageService : IBlobStorageService
    {
        private readonly string BaseUrl = $"https://{Constants.DomainServices}:7294/";

        public async Task<string?> UploadFileToBlobAsync(string strFileName, string contecntType, string ContentData)
        {
            byte[] PlainTextBytes = Encoding.UTF8.GetBytes(ContentData);

            BLobContainer Blob = new()
            {
                contecntType = contecntType,
                ContentData = Convert.ToBase64String(PlainTextBytes),
                strFileName = strFileName
            };

            string? Result = await ServiceHelper.PostItem<string>(BaseUrl, $"api/Storage/containers", Blob);
            return Result;
        }

        public async Task<string?> DownloadFileToBlobAsync(string strFileName)
        {
            return await ServiceHelper.GetItems<string>(BaseUrl, $"api/Storage/containers?FileName={strFileName}");
        }

        public async Task<List<BlobItemDTO>?> GetAllBlobs()
        {
            return await ServiceHelper.GetItems<List<BlobItemDTO>?>(BaseUrl, $"api/Storage/containers/all");
        }
    }
}
