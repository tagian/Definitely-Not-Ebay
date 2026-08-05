namespace DefNotEbay_API.Models
{
    public class Category
    {
        public int CategoryId { get; set; }
        public required string  Name { get; set; }
        public string? Description { get; set; }
        public string? ThumbnailPath { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public ICollection<Item>? Items { get; set; }
    }
}
