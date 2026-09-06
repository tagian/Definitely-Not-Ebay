namespace DefNotEbay_API.DTOs.Item
{
    public class UpdateItemRequest
    {
        public int ItemId { get; set; }
        public required string Name { get; set; }
        public string? Description { get; set; }
        public required decimal Price { get; set; }
        public required int CategoryId { get; set; }
        public string? ThumbnailPath { get; set; }
        public required string Address { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public bool IsActive { get; set; }
    }
}
