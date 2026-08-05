//We need POST Bid
// and maybe get it using its Id

using AutoMapper;
using DefNotEbay_API.Data;
using DefNotEbay_API.DTOs.Bid;
using DefNotEbay_API.DTOs.Message;
using DefNotEbay_API.Models;
using DefNotEbay_API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.AccessControl;


namespace DefNotEbay_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BidsController : ControllerBase
    {
        private readonly IBidService _bidService;
        private readonly IMapper _mapper;
        public BidsController(IBidService bidService, IMapper mapper)
        {
            _mapper = mapper;
            _bidService = bidService;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<BidResponse>> GetBid(int id)
        {
            var bid = await _bidService.GetBid(id);
            if (bid != null)
                return Ok(_mapper.Map<BidResponse>(bid));
            else
                return NotFound();

        }
        [HttpPost]
        [Authorize(Roles = "Admin, Buyer")]

        public async Task<ActionResult> CreateBid(CreateBidRequest req)
        {
            var bid = _mapper.Map<Bid>(req);
            if (await _bidService.CreateBid(bid))
            {
                return CreatedAtAction(nameof(GetBid), new { id = bid.BidId }, _mapper.Map<BidResponse>(bid));
            }
            else
            {
                return BadRequest("Failed to create bid");
            }
        }

        [HttpGet("Auction/{auctionid}")]
        public async Task<ActionResult<BidResponse>> GetBidsByAuction(int auctionid)
        {
            var bids = await _bidService.GetBidsByAuction(auctionid);
            if (bids != null)
                return Ok(_mapper.Map<IEnumerable<BidResponse>>(bids));
            else
                return NotFound();

        }

    }
}
