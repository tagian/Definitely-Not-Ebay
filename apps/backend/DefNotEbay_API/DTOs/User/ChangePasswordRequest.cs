namespace DefNotEbay_API.DTOs.User
{
    public class ChangePasswordRequest
    {
        public int UserId { get; set; }
        public required string NewPassword { get; set; }
        public required string OldPassword { get; set; }
    }
}
