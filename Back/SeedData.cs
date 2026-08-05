using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bogus;
using DefNotEbay_API.Data;
using DefNotEbay_API.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace DefNotEbay_API.Seeding;
//20minutes without auction worker running
public static class SeedData
{
    public static async Task SeedAsync(AppDbContext db, int seed = 1337,
        int sellerCount = 100, int buyerCount = 500, int categoryCount = 10,
        int itemsPerSellerMin = 10, int itemsPerSellerMax = 15,
        int auctionItemRatioPercent = 50, int maxBidsPerAuction = 10)
    {
        // 0) Setup
        Randomizer.Seed = new Random(seed);
        var now = DateTime.UtcNow;

        // ---- 1) USERS + CATEGORIES ----
        var hasher = new PasswordHasher<User>();
        const string plainPassword = "safepassword";

        var userFaker = new Faker<User>()
            .RuleFor(u => u.Name, f => f.Name.FullName())
            .RuleFor(u => u.Email, (f, u) => f.Internet.Email(u.Name))
            .RuleFor(u => u.Phone, f => f.Phone.PhoneNumber())
            .RuleFor(u => u.Address, f => f.Address.StreetAddress())
            .RuleFor(u => u.City, f => f.Address.City())
            .RuleFor(u => u.Region, f => f.Address.State())
            .RuleFor(u => u.PostalCode, f => f.Address.ZipCode())
            .RuleFor(u => u.Country, f => f.Address.Country())
            .RuleFor(u => u.Approved, f => true)
            .RuleFor(u => u.CreatedAt, f => f.Date.PastOffset(2).UtcDateTime);

        var sellers = Enumerable.Range(0, sellerCount).Select(_ =>
        {
            var u = userFaker.Generate();
            u.Role = "Seller";
            u.PasswordHash = hasher.HashPassword(u, plainPassword);
            return u;
        }).ToList();

        var buyers = Enumerable.Range(0, buyerCount).Select(_ =>
        {
            var u = userFaker.Generate();
            u.Role = "Buyer";
            u.PasswordHash = hasher.HashPassword(u, plainPassword);
            return u;
        }).ToList();

        var admin = new User
        {
            Name = "Dev Admin",
            Email = "admin@dev.local",
            Role = "Admin",
            Phone = "6999999990",
            City = "Ath",
            PostalCode = "13121",
            Address = "Athens 1",
            Country = "Gr",
            PasswordHash = "ToBeChanged",
            Approved = true,
            CreatedAt = DateTime.UtcNow,
        };
        admin.PasswordHash = hasher.HashPassword(admin, plainPassword);

        var allUsers = new List<User>();
        allUsers.AddRange(sellers);
        allUsers.AddRange(buyers);
        allUsers.Add(admin);

        var categoryFaker = new Faker<Category>()
            .RuleFor(c => c.Name, f => f.Commerce.Categories(1).First())
            .RuleFor(c => c.Description, f => f.Lorem.Sentence())
            .RuleFor(c => c.ThumbnailPath, f => f.Image.PicsumUrl())
            .RuleFor(c => c.CreatedAt, f => f.Date.PastOffset(1).UtcDateTime);

        var categories = categoryFaker.Generate(categoryCount);

        db.AddRange(allUsers);
        db.AddRange(categories);
        await db.SaveChangesAsync(); // users & categories now have real IDs

        // ---- 2) ITEMS (need real SellerId/CategoryId) ----
        var itemFaker = new Faker<Item>()
            .RuleFor(i => i.Name, f => f.Commerce.ProductName())
            .RuleFor(i => i.Description, f => f.Commerce.ProductDescription())
            .RuleFor(i => i.Price, f => decimal.Parse(f.Commerce.Price(5, 1200)))
            .RuleFor(i => i.ThumbnailPath, f => f.Image.PicsumUrl())
            .RuleFor(i => i.IsActive, f => f.Random.Bool(0.9f))
            .RuleFor(i => i.CreatedAt, f => f.Date.PastOffset(1).UtcDateTime)
            .RuleFor(i => i.Address, f => f.Address.StreetAddress())
            .RuleFor(i => i.Latitude, f => f.Address.Latitude())
            .RuleFor(i => i.Longitude, f => f.Address.Longitude());

        var items = new List<Item>();
        foreach (var seller in sellers)
        {
            int n = Randomizer.Seed.Next(itemsPerSellerMin, itemsPerSellerMax + 1);
            for (int k = 0; k < n; k++)
            {
                var cat = categories[Randomizer.Seed.Next(categories.Count)];
                var it = itemFaker.Generate();

                // Set BOTH nav and FK (FKs are valid since users/categories are saved) :contentReference[oaicite:5]{index=5}
                it.Seller = seller;
                it.SellerId = seller.UserId;
                it.Category = cat;
                it.CategoryId = cat.CategoryId;

                items.Add(it);
            }
        }
        db.AddRange(items);
        await db.SaveChangesAsync(); // items now have ItemId

        var clickTracks = new List<ClickTrack>();

        foreach (var item in items)
        {
            // For each item, simulate a handful of unique buyers who viewed it
            int viewerCount = Randomizer.Seed.Next(1, 8);

            // random unique buyers per item
            var shuffledBuyers = buyers
                .OrderBy(_ => Randomizer.Seed.Next())
                .Take(viewerCount);

            foreach (var buyer in shuffledBuyers)
            {
                // clicks spread with light skew; timestamps consistent with item lifetime
                var clicks = new Faker().Random.Int(1, 100);

                var created = item.CreatedAt.AddDays(new Faker().Random.Int(0, 30));
                if (created > now) created = now.AddDays(-1); // keep historical

                var updated = created.AddDays(new Faker().Random.Int(0, 30));
                if (updated > now) updated = now;

                clickTracks.Add(new ClickTrack
                {
                    UserId = buyer.UserId,                 // persisted user
                    ItemId = item.ItemId,       // persisted item (ClickTrack.ItemId is string)
                    Clicks = clicks,
                    CreatedAt = created,
                    UpdatedAt = updated
                });
            }
        }

        db.AddRange(clickTracks);
        await db.SaveChangesAsync(); // click tracks now stored


        // ---- 3) AUCTIONS (need real ItemId) ----
        var auctionItems = items.Where(_ => Randomizer.Seed.Next(100) < auctionItemRatioPercent).ToList();
        var buyNowOnlyItems = items.Except(auctionItems).ToList();

        var auctionFaker = new Faker<Auction>()
            .RuleFor(a => a.startingPrice, f => (float)f.Random.Double(1, 300))
            .RuleFor(a => a.CreatedAt, f => f.Date.RecentOffset(60).UtcDateTime)
            .RuleFor(a => a.StartingAt, (f, a) => a.CreatedAt.AddMinutes(f.Random.Int(5, 60)))
            .RuleFor(a => a.EndingAt, (f, a) => a.StartingAt.AddHours(f.Random.Int(1, 72)));

        var auctions = new List<Auction>();
        foreach (var item in auctionItems)
        {
            var a = auctionFaker.Generate();
            a.Item = item;          // nav is required; EF will set ItemId too :contentReference[oaicite:6]{index=6}
            auctions.Add(a);
        }
        db.AddRange(auctions);
        await db.SaveChangesAsync(); // auctions now have AuctionId

        // ---- 4) BIDS (need AuctionId & BuyerId) ----
        var bids = new List<Bid>();
        foreach (var a in auctions)
        {
            int count = Randomizer.Seed.Next(0, maxBidsPerAuction + 1);
            decimal current = Math.Max((decimal)Math.Round((double)a.startingPrice, 2), 1m);

            for (int i = 0; i < count; i++)
            {
                var bidder = buyers[Randomizer.Seed.Next(buyers.Count)];
                current += new Faker().Random.Decimal(1, 50);

                var minutes = (int)(a.EndingAt - a.StartingAt).TotalMinutes;
                if (minutes < 1) minutes = 1;

                var bid = new Bid
                {
                    Bidder = bidder,                 // nav
                    BidderId = bidder.UserId,        // FK ok (user saved) :contentReference[oaicite:7]{index=7}
                    Auction = a,                     // nav (AuctionId exists now) :contentReference[oaicite:8]{index=8}
                    Hit = decimal.Round(current, 2),
                    CreatedAt = a.StartingAt.AddMinutes(new Faker().Random.Int(1, minutes)),
                    AuctionId = a.AuctionId
                };
                bids.Add(bid);
            }
        }
        db.AddRange(bids);
        await db.SaveChangesAsync();

        // Pick winners per auction
        var bidsByAuction = bids.GroupBy(b => b.Auction);
        foreach (var g in bidsByAuction)
        {
            var top = g.OrderByDescending(x => x.Hit).FirstOrDefault();
            if (top != null)
            {
                g.Key.Winner = top.Bidder;
                g.Key.WinnerId = top.BidderId;      // nullable FK on Auction :contentReference[oaicite:9]{index=9}
            }
        }
        await db.SaveChangesAsync();

        // ---- 5) ORDERS (from Buy Now, and from ended auctions with winner) ----
        var orders = new List<Order>();

        // Buy Now subset
        foreach (var item in buyNowOnlyItems.Where(_ => new Faker().Random.Bool(0.6f)))
        {
            var buyer = buyers[Randomizer.Seed.Next(buyers.Count)];
            orders.Add(new Order
            {
                Seller = item.Seller,
                SellerId = item.SellerId,
                Buyer = buyer,
                BuyerId = buyer.UserId,
                Item = item,
                ItemId = item.ItemId,
                BuyNow = true,
                AuctionId = null,                  // explicit buy-now path
                DateCreated = now.AddDays(-new Faker().Random.Int(1, 30)),
                DateUpdated = now.AddDays(-new Faker().Random.Int(0, 1))
            });
        }

        // From auctions that ended and have a winner
        foreach (var a in auctions.Where(x => x.WinnerId != null && x.EndingAt <= now))
        {
            orders.Add(new Order
            {
                Seller = a.Item.Seller,
                SellerId = a.Item.SellerId,
                BuyerId = a.WinnerId!.Value,
                Buyer = a.Winner!,
                Item = a.Item,
                ItemId = a.Item.ItemId,
                BuyNow = false,
                AuctionId = a.AuctionId,          // AuctionId is real now :contentReference[oaicite:10]{index=10}
                DateCreated = a.EndingAt.AddMinutes(5),
                DateUpdated = a.EndingAt.AddMinutes(10)
            });
        }

        db.AddRange(orders);
        await db.SaveChangesAsync();

        // ---- 6) CONVERSATIONS (need real user IDs) ----
        var conversations = new List<Conversation>();
        foreach (var o in orders)
        {
            // one conversation per order between seller and buyer
            conversations.Add(new Conversation
            {
                UserAId = o.SellerId,     // users are persisted, IDs are real :contentReference[oaicite:11]{index=11}
                UserBId = o.BuyerId,
                Messages = new List<Message>(),
                CreatedAt = o.DateCreated,
                UpdatedAt = o.DateCreated
            });
        }
        db.AddRange(conversations);
        await db.SaveChangesAsync(); // conversations now have ConversationId

        // ---- 7) MESSAGES (set Conversation nav only; let EF fill ConversationId) ----
        var messages = new List<Message>();

        foreach (var (order, convo) in orders.Zip(conversations))
        {
            void Add(bool fromSeller, string body, int minutesAfter, bool read = true)
            {
                var sentAt = convo.CreatedAt.AddMinutes(minutesAfter);
                messages.Add(new Message
                {
                    SenderId = fromSeller ? order.SellerId : order.BuyerId,
                    ReceipientId = fromSeller ? order.BuyerId : order.SellerId,
                    Content = body,
                    SentAt = sentAt,
                    IsRead = read,
                    ReadAt = read ? sentAt.AddMinutes(3) : null,
                    Conversation = convo,            // only nav; EF sets ConversationId :contentReference[oaicite:12]{index=12}
                    ConversationId = convo.ConversationId
                });
                convo.UpdatedAt = sentAt;
            }

            var itemName = order.Item.Name;
            if (order.BuyNow)
            {
                Add(false, $"Hi! I used Buy Now for '{itemName}'. Is it available today?", 1);
                Add(true, "Yes, it is. After 6pm works for me.", 7);
                Add(false, "Great, see you then!", 12);
            }
            else
            {
                Add(true, $"Congrats! You won '{itemName}'. When would pickup work?", 2);
                Add(false, "Thanks! Can we do Saturday afternoon?", 10);
                Add(true, "Perfect. Sharing address in DM.", 18);
            }
        }

        db.AddRange(messages);
        await db.SaveChangesAsync();
    }
}
