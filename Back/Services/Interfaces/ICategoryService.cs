using DefNotEbay_API.Models;

namespace DefNotEbay_API.Services.Interfaces
{
    public interface ICategoryService
    { 
        Task<IEnumerable<Category>> GetAllCategoriesAsync();
        Task<Category> GetCategoryByIdAsync(int id);
        Task<bool> CreateCategoryAsync(Category category);
        Task<bool> UpdateCategoryAsync(Category updatedCategory);
        Task<bool> DeleteCategoryAsync(Category toBeDeletedCategory);

    }
}
