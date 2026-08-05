using DefNotEbay_API.Models;

namespace DefNotEbay_API.Services.Interfaces
{
    public interface IItemService
    {
        Task<(IEnumerable<Item> Items, int TotalCount)> GetAllItemsAsync(string? search = null, string? orderBy = null, string? orderDir = null, int? categoryId = null, int page = 1,int pageSize = 20);
        Task<Item> GetItemByIdAsync(int id);
        Task<bool> CreateItemAsync(Item item);
        Task<bool> UpdateItemAsync(Item item);
        Task<bool> DeleteItemAsync(int id);
        Task<IEnumerable<Item>> GetItemsBySeller(int sellerid);

    }
}
