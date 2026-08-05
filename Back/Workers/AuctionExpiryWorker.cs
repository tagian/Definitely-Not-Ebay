using DefNotEbay_API.Services.Interfaces;

namespace DefNotEbay_API.Workers
{
    public class AuctionExpiryWorker : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<AuctionExpiryWorker> _log;
        private static readonly TimeSpan Interval = TimeSpan.FromMinutes(1);

        public AuctionExpiryWorker(IServiceScopeFactory scopeFactory, ILogger<AuctionExpiryWorker> log)
        => (_scopeFactory, _log) = (scopeFactory, log);

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var timer = new PeriodicTimer(Interval);
            _log.LogInformation("AuctionExpiryWorker started, interval: {Interval}", Interval);

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _scopeFactory.CreateScope();
                    var svc = scope.ServiceProvider.GetRequiredService<IAuctionExpiryService>();
                    var processed = await svc.CompleteExpiredAuctionsAsync(DateTime.UtcNow);
                    if (processed > 0)
                    {
                        _log.LogInformation("Completed {Count} expired auctions", processed);
                    }
                }
                catch (Exception ex)
                {
                    _log.LogError(ex, "AuctionExpiryWorker tick failed");
                }

                await timer.WaitForNextTickAsync(stoppingToken);
            }


            _log.LogInformation("AuctionExpiryWorker stopping");
        }
    }
}
