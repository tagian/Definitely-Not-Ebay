namespace DefNotEbay_API.DTOs.AdminStats
{
    public class AdminStatsResponse
    {
        public long ItemCount { get; set; }
        public long ActiveItemCount { get; set; }
        public int SellersCount { get; set; }
        public int BuyersCount { get; set; }
        public int UsersCount { get; set; }
        public int CategoriesCount { get; set; }
        public int BuyNowOrdersCount { get; set; }
        public int AuctionOrdersCount { get; set; }
        public int ActiveAuctionCount { get; set; }
        public int CompletedAuctionCount { get; set; }

    }
}
