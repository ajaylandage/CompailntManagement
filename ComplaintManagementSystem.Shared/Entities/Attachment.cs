using System;

namespace ComplaintManagementSystem.Shared.Entities
{
    public class Attachment
    {
        public int Id { get; set; }
        public int ComplaintId { get; set; }
        public string? FileName { get; set; }
        public string? FilePath { get; set; }
        public string? ContentType { get; set; }
        public long Size { get; set; }
        public DateTime UploadedAt { get; set; }
    }
}