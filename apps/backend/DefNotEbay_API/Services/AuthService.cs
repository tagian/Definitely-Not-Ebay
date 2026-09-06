using DefNotEbay_API.Data;
using DefNotEbay_API.DTOs.Auth;
using DefNotEbay_API.Models;
using DefNotEbay_API.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Org.BouncyCastle.Crypto.Generators;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace DefNotEbay_API.Services
{
    public class AuthService : IAuthService
    {
        private readonly IConfiguration _configuration;
        private readonly AppDbContext _context;
        private readonly PasswordHasher<User> _hasher = new();

        public AuthService(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }
        public Task<string> GenerateJwtTokenAsync(User user)
        {
            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var claims = new[]
            {   new Claim(JwtRegisteredClaimNames.Sub, user.UserId.ToString()),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(60), //should add refresh
                signingCredentials: creds
            );

            string tokenString = new JwtSecurityTokenHandler().WriteToken(token);
            return Task.FromResult(tokenString);
        }

        public async Task<AuthResponse> LoginAsync(string email, string password)
        {
            var exists = await _context.Users.AnyAsync(u => u.Email == email);
            if (!exists)
            {
                return new AuthResponse
                {
                    IsSuccess = false,
                    ErrorMessage = "User not found.",
                };
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null || !_hasher.VerifyHashedPassword(user, user.PasswordHash, password).Equals(PasswordVerificationResult.Success))
            {
                return new AuthResponse
                {
                    IsSuccess = false,
                    ErrorMessage = "Invalid credentials.",
                };
            }

            if (user.Approved == false)
            {
                return new AuthResponse
                {
                    IsSuccess = false,
                    ErrorMessage = "User not approved.",
                };
            }

            return new AuthResponse
            {
                IsSuccess = true,
                Token = await GenerateJwtTokenAsync(user),
                Message = "Login successful."
            };
        }

        public async Task<bool> RegisterAsync(User user)
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
    }
}
