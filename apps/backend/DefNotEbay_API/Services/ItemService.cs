using DefNotEbay_API.Data;
using DefNotEbay_API.DTOs.Item;
using DefNotEbay_API.Models;
using DefNotEbay_API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Tls.Crypto.Impl;

namespace DefNotEbay_API.Services
{
    public class ItemService : IItemService
    {
        private readonly AppDbContext _context;

        public ItemService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<bool> CreateItemAsync(Item item)
        {
            item.CreatedAt = DateTime.UtcNow;
            _context.Items.Add(item);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteItemAsync(int id)
        {
            var item = await _context.Items.FindAsync(id);
            if (item == null)
            {
                return false;
            }
            _context.Items.Remove(item);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> CanUserModifyItemAsync(int itemId, int userId)
        {
            var item = await _context.Items.AsNoTracking().FirstOrDefaultAsync(i => i.ItemId == itemId);
            return item?.SellerId == userId;
        }

        public async Task<(IEnumerable<Item> Items, int TotalCount)> GetAllItemsAsync(string? search = null, string? orderBy = null, string? orderDir = null, int? categoryId = null, int page = 1,int pageSize = 20)
        {
            var q = _context.Items
                .Include(i => i.Category)
                .AsQueryable();

            if (categoryId.HasValue)
            {
                q = q.Where(i => i.CategoryId == categoryId.Value);
            }

            if (!string.IsNullOrWhiteSpace(search))
            {
                var s = search.Trim().ToLower();
                q = q.Where(i =>
                    EF.Functions.Like(i.Name.ToLower(), $"%{s}%") ||
                    (i.Description != null && EF.Functions.Like(i.Description.ToLower(), $"%{s}%")));
            }

            var by = (orderBy ?? "createdAt").Trim().ToLower();
            var dir = (orderDir ?? (by == "price" ? "asc" : "desc")).Trim().ToLower();

            q = (by, dir) switch
            {
                ("price", "desc") => q.OrderByDescending(i => i.Price).ThenByDescending(i => i.CreatedAt),
                ("price", _) => q.OrderBy(i => i.Price).ThenByDescending(i => i.CreatedAt),
                ("createdat", "asc") => q.OrderBy(i => i.CreatedAt),
                ("createdat", _) => q.OrderByDescending(i => i.CreatedAt),
                (_, "asc") => q.OrderBy(i => i.CreatedAt),
                _ => q.OrderByDescending(i => i.CreatedAt)
            };

            var total = await q.CountAsync();


            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 1;
            if (pageSize > 100) pageSize = 100;

            var items = await q
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, total);


        }


        public async Task<Item> GetItemByIdAsync(int id)
        {
            var item = await _context.Items.FindAsync(id);
            if (item == null)
            {
                throw new KeyNotFoundException($"Item with ID {id} not found.");
            }
            return item;
        }

        public async Task<bool> UpdateItemAsync(Item item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item), "Item cannot be null.");
            }
            item.UpdatedAt = DateTime.UtcNow;
            _context.Items.Update(item);
            _context.Entry(item).Property(x => x.CreatedAt).IsModified = false;
            _context.Entry(item).Property(x => x.SellerId).IsModified = false;


            return await _context.SaveChangesAsync() > 0;

        }

        public async Task<IEnumerable<Item>> GetItemsBySeller(int sellerid)
        {
            var items = await _context.Items.Where(i => i.SellerId == sellerid).ToListAsync();
            return items;
                    
        }
    }
}
