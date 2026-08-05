using DefNotEbay_API.Models.Enums;

namespace DefNotEbay_API.DTOs.Auction
{
    public class UpdateAuctionRequest
    {
        public int AuctionId { get; set; }
        public AuctionStatus Status { get; set; }
        public float startingPrice { get; set; }
        public DateTime StartingAt { get; set; }
        public DateTime EndingAt { get; set; }
    }
}
