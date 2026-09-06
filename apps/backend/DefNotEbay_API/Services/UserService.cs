using DefNotEbay_API.Data;
using DefNotEbay_API.Models;
using DefNotEbay_API.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace DefNotEbay_API.Services
{
    public class UserService : IUserService
    {
        private readonly AppDbContext _context;
        private readonly PasswordHasher<User> _hasher = new();

        public UserService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<bool> ApproveRole(int userid)
        {
            var user = _context.Users.Find(userid);
            if (user == null)
            {
                return false;
            }
            user.Approved = true;
            user.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ChangePassword(int userid, string newPassword, string oldPassword)
        {
            var user = _context.Users.Find(userid);
            if (user == null)
            {
                return false;
            }
            var result = _hasher.VerifyHashedPassword(user, user.PasswordHash, oldPassword);
            if (result != PasswordVerificationResult.Success)
            {
                return false; 
            }
            user.PasswordHash = _hasher.HashPassword(user, newPassword);
            user.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> CreateUserAsync(User user)
        {
            
            var exists = await _context.Users.AnyAsync(u => u.Email == user.Email);
            if (exists)
            {
                return false;
            }

            user.PasswordHash = _hasher.HashPassword(user, user.PasswordHash);
            _context.Users.Add(user);

            await _context.SaveChangesAsync();
            return true;
            
        }

        public async Task<bool> DeleteUserAsync(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
                return false;

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            return await _context.Users.ToListAsync();
        }

        public async Task<User?> GetUserAsync(int id)
        {
            return await _context.Users.FindAsync(id);
        }

        public async Task<bool> UpdateUserAsync(User user)
        {
            user.UpdatedAt = DateTime.UtcNow;
            _context.Entry(user).State = EntityState.Modified;
            _context.Entry(user).Property(u => u.PasswordHash).IsModified = false;
            _context.Entry(user).Property(u => u.Email).IsModified = false;
            _context.Entry(user).Property(u => u.Role).IsModified = false;
            _context.Entry(user).Property(u => u.CreatedAt).IsModified = false;
            _context.Entry(user).Property(u => u.Approved).IsModified = false;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Users.Any(e => e.UserId == user.UserId))
                {
                    return false;
                }
                else
                {
                    throw;
                }
            }

            return true;
        }
    }
}
