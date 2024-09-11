namespace UploadBobsWorkers.Models
{
    public class BlobItemDTO
    {
        public string Name { get; set; } = null!;
        public DateTimeOffset? CreateOn { get; set; }
        public string ContentType { get; set; } = null!;
        public long ContentLength { get; set; }
        public byte[] ContentHash { get; set; } = [];
    }
}
