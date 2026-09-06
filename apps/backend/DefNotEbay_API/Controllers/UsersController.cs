using AutoMapper;
using DefNotEbay_API.DTOs.User;
using DefNotEbay_API.Extensions;
using DefNotEbay_API.Models;
using DefNotEbay_API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;


namespace DefNotEbay_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IMapper _mapper;

        public UsersController(IUserService userService, IMapper mapper) {
            _userService = userService;
            _mapper = mapper;
        }
        
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserResponse>>> GetAll()
        {
            var users = await _userService.GetAllUsersAsync();
            
            if (users == null || !users.Any())
                return NotFound();

            var response = _mapper.Map<IEnumerable<UserResponse>>(users);

            return Ok(response);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("{id}")]
        public async Task<ActionResult<UserResponse>> GetUser(int id) {

            var user = await _userService.GetUserAsync(id);
            if (user == null)
                return NotFound();
            var response = _mapper.Map<UserResponse>(user);
            return Ok(response);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult<UserResponse>> CreateUser(CreateUserRequest request)
        {
            var user = _mapper.Map<User>(request);
            user.CreatedAt = DateTime.UtcNow;
            user.PasswordHash = request.Password;
            var success = await _userService.CreateUserAsync(user);

            if (!success)
            {
                return Conflict(Conflict("Email already exists."));
            }

            return Ok("User registered successfully.");
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser (int id, UpdateUserRequest request)
        {
            if (id != request.UserId)
                return BadRequest();

            if (request == null)
                return BadRequest();

            var user = _mapper.Map<User>(request);
            var success = await _userService.UpdateUserAsync(user);

            return NoContent();
    
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser (int id)
        {
            var success = await _userService.DeleteUserAsync(id);
            
            if (!success)
                return NotFound();
            
            return NoContent();
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("{id}/approve")]
        public async Task<IActionResult> ApproveRole(int id)
        {
            var success = await _userService.ApproveRole(id);

            if (!success)
                return NotFound();

            return NoContent();
        }

        [Authorize]
        [HttpPost("{id}/change-password")]
        public async Task<IActionResult> ChangePassword(int id, ChangePasswordRequest request)
        {
            if (id != request.UserId)
                return BadRequest("User ID mismatch.");

            var success = await _userService.ChangePassword(id, request.NewPassword, request.OldPassword);

            if (!success) //I should probably Log him out
                return NotFound();

            return NoContent();
        }

        [Authorize]
        [HttpGet("me")]
        [ProducesResponseType(typeof(UserResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> Me()
        {
            var userId = User.GetUserId();

            if (userId is null)
                return Unauthorized();

            var user = await _userService.GetUserAsync(userId.Value);
            if (user is null)
                return Unauthorized(); 

            var me = _mapper.Map<UserResponse>(user);
            return Ok(me);
        }

        [Authorize]
        [HttpPost("me")]
        [ProducesResponseType(typeof(UserResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateMe(UpdateUserRequest request)
        {
            var userId = User.GetUserId();

            if (userId is null)
            {
                return Unauthorized();
            }

            if (userId.Value != request.UserId)
                return Unauthorized();

            if (request == null)
                return BadRequest();

            var user = _mapper.Map<User>(request);
            var success = await _userService.UpdateUserAsync(user);

            var me = _mapper.Map<UserResponse>(user);
            return Ok(me);
        }

    }
}
