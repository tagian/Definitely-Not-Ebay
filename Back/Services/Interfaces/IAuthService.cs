using DefNotEbay_API.DTOs.Auth;
using DefNotEbay_API.Models;

namespace DefNotEbay_API.Services.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponse> LoginAsync(string email, string password);
        Task<bool> RegisterAsync(User user);
        Task<string> GenerateJwtTokenAsync(User user);
    }

}
