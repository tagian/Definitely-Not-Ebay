using DefNotEbay_API.Models;

namespace DefNotEbay_API.Services.Interfaces
{
    public interface ITokenService
    {
        string CreateToken(User user);
    }
}
