using AutoMapper;
using DefNotEbay_API.DTOs.Auth;
using DefNotEbay_API.DTOs.User;
using DefNotEbay_API.Models;
using DefNotEbay_API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DefNotEbay_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IMapper _mapper;

        public AuthController(IAuthService authService, IMapper mapper)
        {
            _authService = authService;
            _mapper = mapper;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(CreateUserRequest request)
        {
            var user = _mapper.Map<User>(request);
            user.CreatedAt = DateTime.UtcNow;
            user.PasswordHash = request.Password; //service should hash the password
            var success = await _authService.RegisterAsync(user);

            if (!success)
            {
                return Conflict(Conflict("Email already exists."));
            }

            return Ok("User registered successfully.");
        }

        [HttpPost("login")]
        [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]

        public async Task<IActionResult> Login(LoginUserRequest request)
        {

            var authResponse = await _authService.LoginAsync(request.Email, request.Password);

            if (!authResponse.IsSuccess)
            {
                return (authResponse.ErrorMessage == "User not approved.") ? Unauthorized("User not approved.") : Unauthorized("Invalid email or password.");
            }
            return Ok(authResponse);

        }


    }
}
