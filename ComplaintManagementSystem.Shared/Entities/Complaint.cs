using System;
using System.Collections.Generic;

namespace ComplaintManagementSystem.Shared.Entities
{
    public class Complaint
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int CategoryId { get; set; }
        public int StatusId { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? LocationAddress { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public string? Priority { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ResolvedAt { get; set; }
        public DateTime? LastUpdatedDate { get; set; }

        // Navigation-like properties for convenience
        public Category? Category { get; set; }
        public ComplaintStatus? Status { get; set; }
        public List<ComplaintUpdate>? Updates { get; set; }
        public List<Attachment>? Attachments { get; set; }
    }
}