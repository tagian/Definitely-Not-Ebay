namespace DefNotEbay_API.Models
{
    public class Item
    {
        public int ItemId { get; set; }                  
        public required string Name { get; set; }             
        public string? Description { get; set; }      
        public required decimal Price { get; set; }           
        public required int CategoryId { get; set; }
        public required Category Category { get; set; }
        public string? ThumbnailPath { get; set; }
        public bool IsActive { get; set; }           
        public DateTime CreatedAt { get; set; }      
        public DateTime? UpdatedAt { get; set; }
        public required int SellerId { get; set; }
        public required User Seller { get; set; }
        public required string Address { get; set; }
        public required double Latitude { get; set; }
        public required double Longitude { get; set; }
        public ICollection<Order> Orders { get; set; } = new List<Order>();

    }
}
