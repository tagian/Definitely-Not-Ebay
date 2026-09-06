namespace DefNotEbay_API.DTOs.Bid
{
    public class BidResponse
    {
        public int? BidId { get; set; }
        public int? BidderId { get; set; }
        public decimal? Hit { get; set; }
        public int? AuctionId { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
}
