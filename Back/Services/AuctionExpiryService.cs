using DefNotEbay_API.Data;
using DefNotEbay_API.Models;
using DefNotEbay_API.Models.Enums;
using DefNotEbay_API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Asn1.X509;


namespace DefNotEbay_API.Services
{
        public class AuctionExpiryService : IAuctionExpiryService
        {
            private readonly AppDbContext _context;
            private readonly IOrderService _orders;
            private readonly ILogger<AuctionExpiryService> _log;

            public AuctionExpiryService(AppDbContext context, IOrderService orders, ILogger<AuctionExpiryService> log)
            => (_context, _orders, _log) = (context, orders, log);


            public async Task<int> CompleteExpiredAuctionsAsync(DateTime UtcNow, int batchSize = 200)
            {
            var utcPlus3 = DateTimeOffset.UtcNow.ToOffset(TimeSpan.FromHours(3));
            var expired = await _context.Auctions
                .Where(a => a.Status == AuctionStatus.Active && a.EndingAt <= utcPlus3.DateTime)
                .OrderBy(a => a.EndingAt)
                .Take(batchSize)
                .Include(a => a.Bids)
                .ToListAsync();

                int processed = 0;
                foreach (var auction in expired)
                {
                    var item = await _context.Items.Where(i => i.ItemId == auction.ItemId).FirstOrDefaultAsync();
                    var topBid = auction!.Bids!
                    .OrderByDescending(b => b.Hit)
                    .ThenBy(b => b.CreatedAt)
                    .FirstOrDefault();

                    bool meetsReserve = topBid != null && (topBid.Hit >= (decimal)auction.startingPrice);

                if (meetsReserve)
                {
                    auction.Status = AuctionStatus.Completed;
                    auction.WinnerId = topBid!.BidderId;

                    var order = new Order
                    {
                        SellerId = item!.SellerId,
                        BuyerId = topBid.BidderId,
                        ItemId = item.ItemId,
                        BuyNow = false,
                        DateCreated = utcPlus3.DateTime,
                        DateUpdated = utcPlus3.DateTime,
                        AuctionId = auction.AuctionId
                    };
                    await _orders.CreateOrder(order);
                }
                else
                {
                    auction.Status = AuctionStatus.Cancelled;
                }


                    processed++;
                }

                await _context.SaveChangesAsync();
                return processed;
            }
        }
    }

