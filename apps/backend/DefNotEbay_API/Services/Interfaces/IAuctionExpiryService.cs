namespace DefNotEbay_API.Services.Interfaces
{
    public interface IAuctionExpiryService
    {
        Task<int> CompleteExpiredAuctionsAsync(DateTime utcNow, int batchSize = 200);
    }
}
