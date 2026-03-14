using System.ComponentModel.DataAnnotations;

namespace ComplaintManagementSystem.Shared.DTOs
{   
    public class LoginRequest
    {
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
        public string? UsernameOrEmail { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        public string? Password { get; set; }
    }
}