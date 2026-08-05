using AutoMapper;
using DefNotEbay_API.Data;
using DefNotEbay_API.DTOs.Auction;
using DefNotEbay_API.DTOs.Item;
using DefNotEbay_API.Models;
using DefNotEbay_API.Services;
using DefNotEbay_API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

namespace DefNotEbay_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuctionsController : ControllerBase
    {
        private readonly IAuctionService _service;
        private readonly IMapper _mapper;

        public AuctionsController(IAuctionService service, IMapper mapper) {
            _service = service;
            _mapper = mapper;
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<AuctionResponse>> GetAuction(int id)
        {
            var auction = await _service.GetAuctionAsync(id);
            if (auction == null)
            {
                return NotFound();
            }
            var response = _mapper.Map<AuctionResponse>(auction);
            return Ok(response);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin, Seller")]
        public async Task<ActionResult> UpdateAuction(UpdateAuctionRequest req, int id)
        {
            if (id != req.AuctionId)
            {
                return BadRequest();
            }
            var auction = _mapper.Map<Auction>(req);
            var success = await _service.UpdateAuctionAsync(auction);
            if (success) { return NoContent(); }
            return NotFound("Auction not found or update failed.");
        }

        [HttpPost]
        [Authorize(Roles = "Admin, Seller")]
        public async Task<ActionResult<bool>> CreateAuction(CreateAuctionRequest req)
        {
            var auction = _mapper.Map<Auction>(req);
            var success = await _service.CreateAuctionAsync(auction);
            if (!success)
            {
                return BadRequest("Failed to create auction.");
            }
            return CreatedAtAction(nameof(GetAuction), new { id = auction.AuctionId }, auction);

        }

        [HttpGet("active/{itemid}")]
        public async Task<ActionResult<AuctionResponse>> GetActiveAuctionByItem(int itemid)
        {
            var auction = await _service.GetActiveAuctionByItem(itemid);
            if (auction == null)
            {
                return NotFound();
            }
            var response = _mapper.Map<AuctionResponse>(auction);
            return Ok(response);
        }

        [HttpGet("seller/mine")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<AuctionResponse>>> GetMine()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? User.FindFirstValue("sub")
            ?? User.FindFirstValue("id");

            if (string.IsNullOrWhiteSpace(userId))
                return Unauthorized();

            var auctions = await _service.GetAuctionsBySeller(Int32.Parse(userId));
            if (auctions == null || !auctions.Any())
                return NotFound();
            var auctionResponses = _mapper.Map<IEnumerable<AuctionResponse>>(auctions);
            return Ok(auctionResponses);
        }

    }
}
