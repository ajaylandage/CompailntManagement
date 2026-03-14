namespace ComplaintManagementSystem.Shared.DTOs
{
    public class UpdateComplaintStatusRequest
    {
        public int ComplaintId { get; set; }
        public int StatusId { get; set; }
        public int UpdatedByUserId { get; set; }
        public string? Note { get; set; }
    }
}