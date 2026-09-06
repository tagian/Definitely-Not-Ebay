namespace DefNotEbay_API.Models
{
    public class Bid
    {
        public int BidId { get; set; }
        public required int BidderId { get; set; }
        public required User Bidder { get; set; }
        public required decimal Hit {  get; set; }
        public required int AuctionId { get; set; }
        public required Auction Auction { get; set; }
        public DateTime CreatedAt { get; set; }

    }
}
