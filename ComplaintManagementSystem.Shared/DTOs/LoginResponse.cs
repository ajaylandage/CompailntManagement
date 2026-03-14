namespace ComplaintManagementSystem.Shared.DTOs
{
    public class LoginResponse
    {
        public string? Token { get; set; }
        public int UserId { get; set; }
        public string? Username { get; set; }
        public int RoleId { get; set; }
    }
}