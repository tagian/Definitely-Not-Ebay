using AutoMapper;
using DefNotEbay_API.DTOs.Item;
using DefNotEbay_API.Models;
using DefNotEbay_API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using DefNotEbay_API.Extensions;

//I need Create, Update, Get, Delete

namespace DefNotEbay_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ItemsController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IItemService _itemService;
        public ItemsController(IMapper mapper, IItemService itemService)
        {
            _mapper = mapper;
            _itemService = itemService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] string? search,
            [FromQuery] string? orderBy,
            [FromQuery] string? orderDir,
            [FromQuery] int? categoryId,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20,
            [FromQuery] bool groupByCategory = false)
        {
            var (items, total) = await _itemService.GetAllItemsAsync(
                search, orderBy, orderDir, categoryId, page, pageSize);

            //pagination info on respHeaders
            var totalPages = (int)Math.Ceiling(total / (double)Math.Max(pageSize, 1));
            var pagination = new
            {
                page,
                pageSize,
                totalCount = total,
                totalPages,
                hasPrevious = page > 1,
                hasNext = page < totalPages
            };
            Response.Headers["X-Pagination"] = JsonSerializer.Serialize(pagination);
            
            var baseUrl = $"{Request.Scheme}://{Request.Host}{Request.Path}";
            var q = Request.Query.ToDictionary(kv => kv.Key, kv => kv.Value.ToString());
            string BuildLink(int p)
            {
                q["page"] = p.ToString();
                q["pageSize"] = pageSize.ToString();
                var query = string.Join("&", q.Where(kv => !string.IsNullOrWhiteSpace(kv.Value))
                                              .Select(kv => $"{Uri.EscapeDataString(kv.Key)}={Uri.EscapeDataString(kv.Value)}"));
                return $"{baseUrl}?{query}";
            }
            var links = new List<string>();
            if (page > 1) links.Add($"<{BuildLink(page - 1)}>; rel=\"prev\"");
            if (page < totalPages) links.Add($"<{BuildLink(page + 1)}>; rel=\"next\"");
            if (links.Any()) Response.Headers["Link"] = string.Join(", ", links);

            if (!groupByCategory)
            {
                var itemResponses = _mapper.Map<IEnumerable<ItemResponse>>(items);
                return Ok(itemResponses);
            }

            var grouped = items
                .GroupBy(i => new { i.CategoryId, CategoryName = i.Category.Name })
                .Select(g => new
                {
                    categoryId = g.Key.CategoryId,
                    categoryName = g.Key.CategoryName,
                    items = _mapper.Map<IEnumerable<ItemResponse>>(g)
                });

            return Ok(grouped);
        }




        [HttpGet("{id}")]
        public async Task<ActionResult<ItemResponse>> GetItem(int id)
        {
            var item = await _itemService.GetItemByIdAsync(id);
            if (item == null)
                return NotFound();
            var itemResponse = _mapper.Map<ItemResponse>(item);
            return Ok(itemResponse);
        }

        [HttpPost]
        [Authorize(Roles = "Seller")]
        public async Task<ActionResult<ItemResponse>> CreateItem(CreateItemRequest request)
        {
            var item = _mapper.Map<Item>(request);
            var success = await _itemService.CreateItemAsync(item);
            if (!success)
                return BadRequest("Item could not be created.");
            var itemResponse = _mapper.Map<ItemResponse>(item);
            return CreatedAtAction(nameof(GetItem), new { id = item.ItemId }, itemResponse);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Seller, Admin")]
        public async Task<IActionResult> UpdateItem(int id, UpdateItemRequest request)
        {
            if (id != request.ItemId)
                return BadRequest("Item ID mismatch.");

            var userId = User.GetUserId();
            if (userId is null)
                return Unauthorized();

            var isAdmin = User.IsInRole("Admin");
            if (!isAdmin)
            {
                var canModify = await _itemService.CanUserModifyItemAsync(id, userId.Value);
                if (!canModify)
                    return Forbid();
            }

            var item = _mapper.Map<Item>(request);
            var success = await _itemService.UpdateItemAsync(item);
            if (!success)
                return NotFound("Item not found or could not be updated.");
            return NoContent();

        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Seller, Admin")]
        public async Task<IActionResult> DeleteItem(int id)
        {
            var userId = User.GetUserId();
            if (userId is null)
                return Unauthorized();

            var isAdmin = User.IsInRole("Admin");
            if (!isAdmin)
            {
                var canModify = await _itemService.CanUserModifyItemAsync(id, userId.Value);
                if (!canModify)
                    return Forbid();
            }

            var success = await _itemService.DeleteItemAsync(id);
            if (!success)
            {
                return NotFound("Item not found or could not be deleted.");
            }
            return NoContent();
        }
        
        [HttpGet("seller/mine")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<ItemResponse>>> GetMine()
        {
            var userId = User.GetUserId();

            if (userId is null)
                return Unauthorized();

            var items = await _itemService.GetItemsBySeller(userId.Value);
            if (items == null || !items.Any())
                return NotFound();
            var itemResponses = _mapper.Map<IEnumerable<ItemResponse>>(items);
            return Ok(itemResponses);
        }
    } 
}
        



