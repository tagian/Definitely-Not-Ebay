namespace DefNotEbay_API.DTOs.Item
{
    public class CreateItemRequest
    {
        public required string Name { get; set; }
        public string? Description { get; set; }
        public required decimal Price { get; set; }
        public required int CategoryId { get; set; }
        public string? ThumbnailPath { get; set; }
        public bool IsActive { get; set; }
        public required string Address { get; set; }
        public required double Latitude { get; set; }
        public required double Longitude { get; set; }
        public required int SellerId { get; set; }
    }
}
