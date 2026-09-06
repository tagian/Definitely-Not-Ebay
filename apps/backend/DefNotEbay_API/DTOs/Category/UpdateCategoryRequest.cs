namespace DefNotEbay_API.DTOs.Category
{
    public class UpdateCategoryRequest
    {
        public int CategoryId { get; set; }
        public required string Name { get; set; }
        public string? Description { get; set; }
        public string? ThumbnailPath { get; set; }

    }
}
