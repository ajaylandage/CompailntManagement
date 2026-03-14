using System;

namespace ComplaintManagementSystem.Shared.Entities
{
    public class ComplaintUpdate
    {
        public int Id { get; set; }
        public int ComplaintId { get; set; }
        public int UpdatedByUserId { get; set; }
        public int StatusId { get; set; }
        public string? Note { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}