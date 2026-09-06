using DefNotEbay_API.Models.Enums;

namespace DefNotEbay_API.DTOs.Auction
{
    public class AuctionResponse
    {
        public int AuctionId { get; set; }
        public int ItemId { get; set; }       
        public int? WinnerId { get; set; }
        public AuctionStatus Status { get; set; }
        public DateTime StartingAt { get; set; }
        public DateTime EndingAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public float startingPrice { get; set; }

    }
}
