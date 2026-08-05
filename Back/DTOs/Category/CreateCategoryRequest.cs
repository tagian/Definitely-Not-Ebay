namespace DefNotEbay_API.DTOs.Category
{
    public class CreateCategoryRequest
    {
        public required string Name { get; set; }
        public string? Description { get; set; }
        public string? ThumbnailPath { get; set; }
    }
}
