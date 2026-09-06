using DefNotEbay_API.DTOs.Recommendation;
using DefNotEbay_API.DTOs.UserRecommendation;
using DefNotEbay_API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DefNotEbay_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RecommendationsController : ControllerBase
    {
        private readonly IRecommendationService _service;

        public RecommendationsController(IRecommendationService service)
        {
            _service = service;
        }

        // GET /api/recommendations/{userId}?top=30
        [HttpGet("{userId:int}")]
        [Authorize(Roles = "Buyer")]
        public async Task<ActionResult<IEnumerable<UserRecommendationResponse>>> GetForUser(int userId,[FromQuery] int top = 30)
        {
            var items = await _service.GetTopRecommendationsForUserAsync(userId, top);

            var response = items.Select(r => new UserRecommendationResponse
            {
                ItemId = r.ItemId
            });

            return Ok(response);
        }
    }
}