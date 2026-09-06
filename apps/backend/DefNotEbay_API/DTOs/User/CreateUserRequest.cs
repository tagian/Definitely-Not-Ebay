namespace DefNotEbay_API.DTOs.User
{
    public class CreateUserRequest
    {
        public required string Name { get; set; }
        public required string Email { get; set; }
        public required string Password { get; set; }
        public required string Phone { get; set; }
        public required string Address { get; set; }
        public required string City { get; set; }
        public string? Region { get; set; }
        public required string PostalCode { get; set; }
        public required string Country { get; set; }
        public required string Role { get; set; }
    }
}
