using DefNotEbay_API.Models.Enums;

namespace DefNotEbay_API.DTOs.Auction
{
    public class CreateAuctionRequest
    {
        public int ItemId { get; set; }
        public float startingPrice { get; set; }
        public AuctionStatus Status { get; set; }
        public DateTime StartingAt { get; set; }
        public DateTime EndingAt { get; set; }

    }
}
