using DefNotEbay_API.Models.Enums;

namespace DefNotEbay_API.Models
{
    public class Auction
    {
        public int AuctionId { get; set; }
        public int ItemId { get; set; }
        public required Item Item { get; set; }
        public required float startingPrice { get; set; }
        public ICollection<Bid>? Bids { get; set; }
        public int? WinnerId { get; set; }
        public User? Winner { get; set; }
        public AuctionStatus Status { get; set; }
        public DateTime StartingAt { get; set; }
        public DateTime EndingAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
