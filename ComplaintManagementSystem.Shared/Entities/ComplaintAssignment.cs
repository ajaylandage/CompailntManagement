using System;

namespace ComplaintManagementSystem.Shared.Entities
{
    public class ComplaintAssignment
    {
        public int Id { get; set; }
        public int ComplaintId { get; set; }
        public int AssignedToUserId { get; set; }
        public int AssignedByUserId { get; set; }
        public DateTime AssignedAt { get; set; }
        public DateTime LastAssignmentDate { get; set; }
    }
}