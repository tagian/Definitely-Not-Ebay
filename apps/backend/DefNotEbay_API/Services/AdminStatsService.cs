using DefNotEbay_API.Data;
using DefNotEbay_API.DTOs.AdminStats;
using DefNotEbay_API.Models;
using DefNotEbay_API.Models.Enums;
using DefNotEbay_API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DefNotEbay_API.Services
{
    public sealed class AdminStatsService : IAdminStatsService
    {
        private readonly AppDbContext _context;

        public AdminStatsService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<AdminStatsResponse> GetAdminStatsAsync()
        {
            var itemCount = await _context.Items.LongCountAsync();
            var activeItemCount = await _context.Items.LongCountAsync(i => i.IsActive);
            var sellersCount = await _context.Users.Where(u => u.Role == "Seller").CountAsync();
            var buyersCount = await _context.Users.Where(u => u.Role == "Buyer").CountAsync();
            var usersCount = await _context.Users.CountAsync();
            var categoriesCount = await _context.Categories.CountAsync();
            var buyNowOrdersCount = await _context.Orders.Where(o => o.BuyNow == true).CountAsync();
            var auctionOrdersCount = await _context.Orders.Where(o => o.BuyNow == false).CountAsync();
            var activeAuctionCount = await _context.Auctions.Where(a => a.Status == AuctionStatus.Active).CountAsync();
            var completedAuctionCount = await _context.Auctions.Where(a => a.Status == AuctionStatus.Completed).CountAsync();

            return new AdminStatsResponse
            {
                ItemCount = itemCount,
                ActiveItemCount = activeItemCount,
                SellersCount = sellersCount,
                BuyersCount = buyersCount,
                UsersCount = usersCount,
                CategoriesCount = categoriesCount,
                BuyNowOrdersCount = buyNowOrdersCount,
                AuctionOrdersCount = auctionOrdersCount,
                ActiveAuctionCount = activeAuctionCount,
                CompletedAuctionCount = completedAuctionCount
            };
        }

    }

}
