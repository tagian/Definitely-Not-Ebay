using DefNotEbay_API.Models;

namespace DefNotEbay_API.Services.Interfaces
{
    public interface IAuctionService
    {
        Task<bool> CreateAuctionAsync(Auction auction);
        Task<Auction?> GetAuctionAsync(int id);
        Task<bool> UpdateAuctionAsync(Auction auction);
        Task<bool> DeleteAuctionAsync(int id);
        Task<bool> CanUserModifyAuctionAsync(int auctionId, int userId);
        Task<Auction?> GetActiveAuctionByItem(int itemid);
        Task<IEnumerable<Auction?>> GetAuctionsBySeller(int sellerid);
    }
}
