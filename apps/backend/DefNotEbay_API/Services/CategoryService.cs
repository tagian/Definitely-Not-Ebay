using DefNotEbay_API.Data;
using DefNotEbay_API.Models;
using DefNotEbay_API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DefNotEbay_API.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly AppDbContext _context;
        public CategoryService(AppDbContext context)
        {
            _context = context;
        }
        public async Task<bool> CreateCategoryAsync(Category category)
        {
            category.CreatedAt = DateTime.UtcNow;
            _context.Categories.Add(category);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteCategoryAsync(Category toBeDeletedCategory)
        {
            if (toBeDeletedCategory == null)
            {
                return false;
            }
            _context.Categories.Remove(toBeDeletedCategory);
            return await _context.SaveChangesAsync() > 0;

        }

        public async Task<IEnumerable<Category>> GetAllCategoriesAsync()
        {
            var categories = await _context.Categories.ToListAsync();
            return categories;
        }

        public async Task<Category> GetCategoryByIdAsync(int id)
        {
            var category = await _context.Categories.FirstOrDefaultAsync(c => c.CategoryId == id);
            if (category == null)
            {
                throw new KeyNotFoundException($"Category with ID {id} not found.");
            }
            return category;
        }

        public async Task<bool> UpdateCategoryAsync(Category updatedCategory)
        {
            if (updatedCategory == null )
            {
                return false;
            }

            updatedCategory.UpdatedAt = DateTime.UtcNow;
            _context.Entry(updatedCategory).State = EntityState.Modified;
            _context.Entry(updatedCategory).Property(u => u.CreatedAt).IsModified = false;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Categories.Any(e => e.CategoryId == updatedCategory.CategoryId))
                {
                    return false;
                }
                else
                {
                    throw;
                }
            }

            return true;

        }
    }
}
