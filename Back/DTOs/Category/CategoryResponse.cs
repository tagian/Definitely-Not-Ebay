namespace DefNotEbay_API.DTOs.Category
{
    public class CategoryResponse
    {
        public int CategoryId { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? ThumbnailPath { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        
    }
}
