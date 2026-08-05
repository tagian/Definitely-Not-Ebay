namespace DefNotEbay_API.DTOs.Bid
{
    public class CreateBidRequest
    {
        public required int BidderId { get; set; }
        public required decimal Hit { get; set; }
        public required int AuctionId { get; set; }
    }
}
