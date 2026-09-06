using AutoMapper;
using DefNotEbay_API.Data;
using DefNotEbay_API.Models;
using DefNotEbay_API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DefNotEbay_API.Services
{
    public class AuctionService : IAuctionService
    {
        private readonly AppDbContext _context;
        public AuctionService(AppDbContext context)
        {
            _context = context;
            
        }
        public async Task<bool> CreateAuctionAsync(Auction auction)
        {
            auction.CreatedAt = DateTime.UtcNow;
            _context.Auctions.Add(auction);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAuctionAsync(int id)
        {
            var auction = await _context.Auctions.FindAsync(id);
            if (auction == null)
            {
                return false;
            }
            _context.Auctions.Remove(auction);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> CanUserModifyAuctionAsync(int auctionId, int userId)
        {
            var auction = await _context.Auctions.AsNoTracking().FirstOrDefaultAsync(a => a.AuctionId == auctionId);
            return auction?.Item.SellerId == userId;
        }

        public async Task<Auction?> GetAuctionAsync(int id)
        {
            var auction = await _context.Auctions.FindAsync(id);
            if (auction == null)
            {
                return null;
            }
            return auction;
        }

        public async Task<bool> UpdateAuctionAsync(Auction auction)
        {
            if (auction == null)
            {
                throw new ArgumentNullException(nameof(auction), "Auction cannot be null.");
            }
            auction.UpdatedAt = DateTime.UtcNow;
            _context.Auctions.Update(auction);
            _context.Entry(auction).Property(x => x.CreatedAt).IsModified = false;
            _context.Entry(auction).Property(x => x.ItemId).IsModified = false;
            _context.Entry(auction).Property(x => x.WinnerId).IsModified = false;
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<Auction?>  GetActiveAuctionByItem(int itemid)
        {
            var auction = await _context.Auctions
                .Where(a => a.ItemId == itemid
                     && (a.Status == Models.Enums.AuctionStatus.Active))
            .FirstOrDefaultAsync();

            return auction;
        }

        public async Task<IEnumerable<Auction?>> GetAuctionsBySeller(int sellerId)
        {
            var auctions = await (from a in _context.Auctions
                                  join i in _context.Items on a.ItemId equals i.ItemId
                                  where i.SellerId == sellerId
                                  select a)
                                 .ToListAsync();
            return auctions;
        }
    }
}
