using AutoMapper;
using DefNotEbay_API.DTOs.Order;
using DefNotEbay_API.Models;
using DefNotEbay_API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DefNotEbay_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly IMapper _mapper;

        public OrdersController(IOrderService orderService, IMapper mapper)
        {
            _orderService = orderService;
            _mapper = mapper;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<OrderResponse>>> GetAll()
        {
            var orders = await _orderService.GetAllOrders();
            if (orders == null || !orders.Any())
            {
                return NotFound();
            }

            return Ok(_mapper.Map<IEnumerable<OrderResponse>>(orders));
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<OrderResponse>> GetOrder(int id)
        {
            var order = await _orderService.GetOrder(id);

            if (order != null)
                return Ok(_mapper.Map<OrderResponse>(order));

            return NotFound();
        }

        [HttpPost]
        [Authorize(Roles = "Admin, Buyer")]
        public async Task<ActionResult<OrderResponse>> CreateOrder(CreateOrderRequest req)
        {
            var order = _mapper.Map<Order>(req);
            var success = await _orderService.CreateOrder(order);

            if (!success)
            {
                return BadRequest("order could not be created.");
            }
            return CreatedAtAction(nameof(GetOrder), new { id = order.OrderId }, _mapper.Map<OrderResponse>(order));

        }

        [Authorize]
        [HttpGet("mine")]
        public async Task<ActionResult<IEnumerable<OrderResponse>>> GetMine()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? User.FindFirstValue("sub")
            ?? User.FindFirstValue("id");

            if (string.IsNullOrWhiteSpace(userId))
                return Unauthorized();

            var orders = await _orderService.GetUserOrders(Int32.Parse(userId));
            if (orders == null || !orders.Any())
            {
                return NotFound();
            }

            return Ok(_mapper.Map<IEnumerable<OrderResponse>>(orders));
        }


    }
}
