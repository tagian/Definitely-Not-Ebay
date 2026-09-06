using AutoMapper;
using DefNotEbay_API.Data;
using DefNotEbay_API.DTOs.Category;
using DefNotEbay_API.Models;
using DefNotEbay_API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

//We need create, update, get, delete

namespace DefNotEbay_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryService _categoryService;
        private readonly IMapper _mapper;

        public CategoriesController(ICategoryService categoryService, IMapper mapper) {
            _categoryService = categoryService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CategoryResponse>>> GetAll()
        {
            var categories = await _categoryService.GetAllCategoriesAsync();
            if (categories == null || !categories.Any())
            {
                return NotFound();
            }

            return Ok(_mapper.Map<IEnumerable<CategoryResponse>>(categories));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CategoryResponse>> GetCategory(int id)
        {
            var category = await _categoryService.GetCategoryByIdAsync(id);

            if (category != null)
                return Ok(_mapper.Map<CategoryResponse>(category));

            return NotFound();
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]

        public async Task<ActionResult<CategoryResponse>> CreateCategory(CreateCategoryRequest req)
        {
            var category = _mapper.Map<Category>(req);
            var success = await _categoryService.CreateCategoryAsync(category);

            if (!success)
            {
                return BadRequest("Category could not be created.");
            }
            return CreatedAtAction(nameof(GetCategory), new { id = category.CategoryId }, _mapper.Map<CategoryResponse>(category));

        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]

        public async Task<ActionResult> UpdateCategory(UpdateCategoryRequest req, int id)
        {
            if (id != req.CategoryId)
            {
                return BadRequest();
            }

            if (req == null)
            {
                return BadRequest();
            }

            var category = _mapper.Map<Category>(req);
            var success = await _categoryService.UpdateCategoryAsync(category);
            if (!success)
            {
                return NotFound();
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]

        public async Task<ActionResult> DeleteCategory(int id)
        {
            var category = await _categoryService.GetCategoryByIdAsync(id);
            var success = await _categoryService.DeleteCategoryAsync(category);

            if (!success)
            {
                return NotFound();
            }
            return NoContent();
        }


    }
}
