using DefNotEbay_API.Models;

namespace DefNotEbay_API.Services.Interfaces
{
    public interface IUserService
    {
        Task<IEnumerable<User>> GetAllUsersAsync();
        Task<User?> GetUserAsync(int id);
        Task<bool> UpdateUserAsync(User user);
        Task <bool> CreateUserAsync(User user);
        Task<bool> DeleteUserAsync(int id);
        Task<bool> ApproveRole(int id);
        Task<bool> ChangePassword(int id, string newPassword, string oldPassword);
    }
}
