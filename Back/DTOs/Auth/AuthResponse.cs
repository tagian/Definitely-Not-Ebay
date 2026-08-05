namespace DefNotEbay_API.DTOs.Auth
{
    public class AuthResponse
    {
        public string? Token { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime? ExpiresAt { get; set; }
        public string? Message { get; set; }
        public bool IsSuccess { get; set; } = true;
        public string? ErrorMessage { get; set; }
    }
}
