using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ComplaintManagementSystem.Shared.DTOs
{
    public class CreateComplaintRequest
    {
        public int UserId { get; set; }

        [Required(ErrorMessage = "Category is required.")]
        public int? CategoryId { get; set; }

        [Required(ErrorMessage = "Title is required.")]
        public string? Title { get; set; }

        [Required(ErrorMessage = "Description is required.")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Address is required.")]
        public string? LocationAddress { get; set; }

        [Required(ErrorMessage = "Priority is required.")]
        public string? Priority { get; set; }

        public double? Latitude { get; set; }

        public double? Longitude { get; set; }

        // For uploads we'll accept metadata here; actual file uploading handled by API endpoints
        public List<string>? AttachmentPaths { get; set; }
    }
}