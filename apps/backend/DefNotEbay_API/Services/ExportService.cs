using Bogus.Bson;
using DefNotEbay_API.Data;
using DefNotEbay_API.DTOs.Export;
using DefNotEbay_API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Text;
using System.Text.Json;
using System.Xml.Serialization;

namespace DefNotEbay_API.Services
{
    public sealed class ExportService : IExportService
    {
        private readonly AppDbContext _context;

        public ExportService(AppDbContext context) => _context = context;

        public async Task<ExportResult> ExportAsync(string format = "json", int? sellerId = null, DateTime? start = null, DateTime? end = null)
        {
            var baseQuery =
                from a in _context.Auctions
                join i in _context.Items on a.ItemId equals i.ItemId
                join c in _context.Categories on i.CategoryId equals c.CategoryId into cg
                from c in cg.DefaultIfEmpty()
                join s in _context.Users on i.SellerId equals s.UserId into sg
                from s in sg.DefaultIfEmpty()
                select new
                {
                    Auction = a,
                    Item = i,
                    CategoryName = c != null ? c.Name : null,
                    Seller = s
                };

            if (sellerId.HasValue)
                baseQuery = baseQuery.Where(x => x.Item.SellerId == sellerId.Value);

            if (start.HasValue)
                baseQuery = baseQuery.Where(x => x.Auction.CreatedAt >= start.Value);

            if (end.HasValue)
                baseQuery = baseQuery.Where(x => x.Auction.EndingAt <= end.Value);

            var baseRows = await baseQuery.ToListAsync();

            var auctionIds = baseRows.Select(x => x.Auction.AuctionId).Distinct().ToList();

            var bidRows =
                await (from b in _context.Bids
                       join u in _context.Users on b.BidderId equals u.UserId into ug
                       from u in ug.DefaultIfEmpty()
                       where auctionIds.Contains(b.AuctionId)
                       select new
                       {
                           Bid = b,
                           Bidder = u
                       })
                .ToListAsync();

            var bidsByAuction = bidRows
                .GroupBy(x => x.Bid.AuctionId)
                .ToDictionary(
                    g => g.Key,
                    g => g.OrderBy(x => x.Bid.CreatedAt).ToList());
            var envelope = new ExportItemsDto();

            foreach (var row in baseRows)
            {
                var a = row.Auction;
                var i = row.Item;

                var categories = new List<string>();
                if (!string.IsNullOrWhiteSpace(row.CategoryName))
                    categories.Add(row.CategoryName);

                bidsByAuction.TryGetValue(a.AuctionId, out var bids);
                bids ??= new();

                var numberOfBids = bids.Count;
                var firstBidAmount = numberOfBids > 0 ? bids.First().Bid.Hit : i.Price;
                var currentAmount = numberOfBids > 0 ? bids.Last().Bid.Hit : i.Price;

                envelope.Items.Add(new ExportItemDto
                {
                    ItemID = i.ItemId.ToString(CultureInfo.InvariantCulture),
                    Name = i.Name,
                    Categories = categories,
                    Currently = AsMoney(currentAmount),
                    BuyPrice = AsMoney(i.Price),
                    FirstBid = AsMoney(firstBidAmount),
                    NumberOfBids = numberOfBids,
                    Bids = bids.Select(b => new ExportBidDto
                    {
                        Bidder = new ExportBidderDto
                        {
                            UserID = (b.Bidder?.UserId ?? b.Bid.BidderId).ToString(CultureInfo.InvariantCulture),
                            Location = b.Bidder?.Address,
                            Country = b.Bidder?.Country
                        },
                        Time = AsDateTime(b.Bid.CreatedAt),
                        Amount = AsMoney(b.Bid.Hit)
                    }).ToList(),
                    Location = new ExportLocationDto
                    {
                        Latitude = AsCoordinates(i.Latitude),
                        Longitude = AsCoordinates(i.Longitude),
                    },
                    Started = AsDateTime(a.StartingAt),
                    Ends = AsDateTime(a.EndingAt),
                    Seller = new ExportSellerDto
                    {
                        UserID = i.SellerId.ToString(CultureInfo.InvariantCulture),
                    },
                    Description = i.Description
                });
            }

            // ------------ Serialize ------------
            format = (format ?? "json").Trim().ToLowerInvariant();
            var ts = DateTime.UtcNow.ToString("yyyyMMdd_HHmmss", CultureInfo.InvariantCulture);

            if (format == "xml")
            {
                var serializer = new XmlSerializer(typeof(ExportItemsDto));
                var ns = new XmlSerializerNamespaces();
                ns.Add(string.Empty, string.Empty);

                using var ms = new MemoryStream();
                using (var writer = new StreamWriter(ms, new UTF8Encoding(encoderShouldEmitUTF8Identifier: false)))
                {
                    serializer.Serialize(writer, envelope, ns);
                }
                return new ExportResult
                {
                    Content = ms.ToArray(),
                    ContentType = "application/xml",
                    FileName = $"export_{ts}.xml"
                };
            }
            else
            {
                var json = JsonSerializer.Serialize(envelope, new JsonSerializerOptions
                {
                    WriteIndented = true
                });
                return new ExportResult
                {
                    Content = Encoding.UTF8.GetBytes(json),
                    ContentType = "application/json",
                    FileName = $"export_{ts}.json"
                };
            }
        }
        private static string AsMoney(decimal? amount)
            => amount.HasValue ? amount.Value.ToString("0.00", CultureInfo.InvariantCulture) : "0.00";

        private static string AsDateTime(DateTime? dt)
            => dt.HasValue ? dt.Value.ToString("yyyy-MM-ddTHH:mm:ssZ", CultureInfo.InvariantCulture) : "";
        private static string? AsCoordinates(double? value)
        {
            return value.HasValue
                ? value.Value.ToString("0.000000", CultureInfo.InvariantCulture)
                : null;
        }


    }
}
