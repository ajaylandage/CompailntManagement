using System;
using System.Collections.Generic;
using ComplaintManagementSystem.Shared.Entities;

namespace ComplaintManagementSystem.Shared.DTOs
{
    public class ComplaintDetailsResponse
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int CategoryId { get; set; }
        public int StatusId { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? LocationAddress { get; set; }
        public string? Priority { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public string? CategoryName { get; set; }
        public string? StatusName { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ResolvedAt { get; set; }
        public List<ComplaintUpdateResponse>? Updates { get; set; }
        public List<Attachment>? Attachments { get; set; }
    }
}