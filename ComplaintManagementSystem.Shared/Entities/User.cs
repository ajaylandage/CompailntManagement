using System;

namespace ComplaintManagementSystem.Shared.Entities
{
    public class User
    {
        public int Id { get; set; } // maps to UserID
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? PasswordHash { get; set; }
        public int RoleId { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}