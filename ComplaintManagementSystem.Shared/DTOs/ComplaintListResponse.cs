using System;
using System.Collections.Generic;

namespace ComplaintManagementSystem.Shared.DTOs
{
    public class ComplaintListResponse
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? CategoryName { get; set; }
        public string? StatusName { get; set; }
        public string? Priority { get; set; }
        public DateTime CreatedAt { get; set; }
        public int UserId { get; set; }
    }
}