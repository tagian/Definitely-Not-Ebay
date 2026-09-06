using DefNotEbay_API.DTOs.ClickTrack;
using DefNotEbay_API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace DefNotEbay_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TrackClickController : ControllerBase
    {
        private readonly IClickTrackService _service;

        public TrackClickController(IClickTrackService service)
        {
            _service = service;
        }

        [HttpPost]
        [Authorize(Roles = "Buyer")]
        public async Task<ActionResult> RecordClick(ClickTrackRequest req)
        {
            var track = await _service.RecordClickAsync(req.UserId, req.ItemId);
            return Ok();

        }

    }
}
