using DefNotEbay_API.DTOs.Bid;
using DefNotEbay_API.Models;
using Microsoft.AspNetCore.Mvc;

namespace DefNotEbay_API.Services.Interfaces
{
    public interface IBidService
    {
        Task<Bid> GetBid(int id);
        Task<bool> CreateBid(Bid bid);
        Task<IEnumerable<Bid>> GetBidsByAuction(int auctionid);
    }
}
