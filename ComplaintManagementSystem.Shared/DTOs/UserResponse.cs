namespace ComplaintManagementSystem.Shared.DTOs
{
    public class UserResponse
    {
        public int UserId { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? PhoneNumber { get; set; }
        public int RoleId { get; set; }
    }
}
