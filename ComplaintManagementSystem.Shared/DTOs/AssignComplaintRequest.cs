namespace ComplaintManagementSystem.Shared.DTOs
{
    public class AssignComplaintRequest
    {
        public int ComplaintId { get; set; }
        public int AssignedToUserId { get; set; }
        public int AssignedByUserId { get; set; }
    }
}