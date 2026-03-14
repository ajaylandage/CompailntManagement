using System;

namespace ComplaintManagementSystem.Shared.DTOs
{
    public class ComplaintUpdateResponse
    {
        public int Id { get; set; }
        public int ComplaintId { get; set; }
        public int UpdatedByUserId { get; set; }
        public int StatusId { get; set; }
        public string? UpdateMessage { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
