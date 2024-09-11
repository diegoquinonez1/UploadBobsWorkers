using UploadBobsWorkers.Common;
using UploadBobsWorkers.Interfaces;
using UploadBobsWorkers.Models;

namespace UploadBobsWorkers
{
    public class Worker(ILogger<Worker> logger, IBlobStorageService blobStorageService) : BackgroundService
    {
        private readonly ILogger<Worker> _logger = logger;
        private readonly IBlobStorageService blobService = blobStorageService;

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            bool Finished = false;
            while (!stoppingToken.IsCancellationRequested && !Finished)
            {
                if (_logger.IsEnabled(LogLevel.Information))
                {
                    List<BlobItemDTO>? Blobs  = await blobService.GetAllBlobs();

                    if (Blobs != null)
                    {
                        Console.WriteLine("Numero de BLob: " + Blobs.Count);
                        long Count = 1;
                        foreach (BlobItemDTO Blob in Blobs)
                        {
                            try
                            {
                                //string ContentBlobName = "0124659289_0005169387_8091185000.txt";
                                string ContentBlobName = Blob.Name;
                                Console.WriteLine("BLob No: " + Count);
                                Count++;
                                string? BlobContent = await GetBlobContent($"{ContentBlobName}");
                                if (BlobContent != null)
                                {
                                    string compressedPdfBase64 = PdfCompressor.CompressPdfBase64(BlobContent);
                                    await UploadBlobContent($"{ContentBlobName}", compressedPdfBase64);
                                }
                            }
                            catch(Exception e)
                            {

                                Console.WriteLine("BLob: " + Blob.Name);
                                Console.WriteLine("Error: : " + e.Message);
                                Console.WriteLine("Error: : " + Blob.CreateOn);
                            }
                        }
                        Finished = true;
                    }

                    Console.WriteLine();
                    Console.WriteLine("Finished");
                    Console.WriteLine();
                }
                await Task.Delay(1000, stoppingToken);
            }
        }

        private async Task<string?> GetBlobContent(string FileName) => await blobService.DownloadFileToBlobAsync(FileName);
        private async Task UploadBlobContent(string BlobName, string DocumentInBase64) => await blobService.UploadFileToBlobAsync(BlobName, "text/plain", DocumentInBase64);
    }
}
