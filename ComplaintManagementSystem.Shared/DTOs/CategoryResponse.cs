namespace ComplaintManagementSystem.Shared.DTOs
{
    public class CategoryResponse
    {
        public int CategoryID { get; set; }
        public string? CategoryName { get; set; }
        public string? Description { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}