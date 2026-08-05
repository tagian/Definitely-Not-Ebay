namespace DefNotEbay_API.Models
{
    public class User
    {
        public int UserId { get; set; }
        public required string Name { get; set; }
        public required string Email { get; set; }
        public required string PasswordHash { get; set; }
        public required string Phone { get; set; }
        public required string Address { get; set; }
        public required string City { get; set; }
        public string? Region { get; set; }
        public required string PostalCode { get; set; }
        public required string Country { get; set; }
        public required string Role { get; set; }
        public bool Approved { get; set; } = false;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public ICollection<Order> Sales { get; set; } = new List<Order>();
        public ICollection<Order> Purchases { get; set; } = new List<Order>();

    }
}
