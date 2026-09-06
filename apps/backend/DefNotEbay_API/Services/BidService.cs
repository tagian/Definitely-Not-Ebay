using DefNotEbay_API.Data;
using DefNotEbay_API.Models;
using DefNotEbay_API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DefNotEbay_API.Services
{
    public class BidService : IBidService
    {
        private readonly AppDbContext _context;

        public BidService(AppDbContext context)
        {
            _context = context;
        }
        public async Task<bool> CreateBid(Bid bid)
        {
            bid.CreatedAt = DateTime.UtcNow;
            _context.Bids.Add(bid);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<Bid> GetBid(int id)
        {
            var bid = await _context.Bids.FindAsync(id);
            if (bid == null)
            {
                throw new KeyNotFoundException($"Bid with ID {id} not found.");
            }
            return bid;
        }

        public async Task<IEnumerable<Bid>> GetBidsByAuction(int auctionid)
        {
            var bids = await _context.Bids.Where(b => b.AuctionId == auctionid).ToListAsync();
            
            return bids;
        }
    }
}
