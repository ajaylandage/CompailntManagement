using System;
using System.Collections.Generic;

namespace ComplaintManagementSystem.Shared.DTOs
{
    public class RecentComplaintDto
    {
        public int ComplaintId { get; set; }
        public string? Title { get; set; }
        public string? CategoryName { get; set; }
        public string? StatusName { get; set; }
        public DateTime CreatedDate { get; set; }
    }

    public class DashboardResponse
    {
        public int TotalComplaints { get; set; }
        public int PendingComplaints { get; set; }
        public int InProgressComplaints { get; set; }
        public int ResolvedComplaints { get; set; }
        public List<RecentComplaintDto>? RecentComplaints { get; set; }
    }
}
