using System.ComponentModel.DataAnnotations;

namespace ComplaintManagementSystem.Shared.DTOs
{
    public class CategoryRequest
    {
        [Required]
        public string CategoryName { get; set; } = string.Empty;
        
        public string? Description { get; set; }
        
        public bool IsActive { get; set; } = true;
    }
}
