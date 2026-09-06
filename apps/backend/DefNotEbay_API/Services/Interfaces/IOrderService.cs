using DefNotEbay_API.Models;

namespace DefNotEbay_API.Services.Interfaces
{
    public interface IOrderService
    {
        Task<IEnumerable<Order>> GetAllOrders();
        Task<Order> GetOrder(int id);
        Task<bool> CreateOrder(Order order);
        Task<IEnumerable<Order>> GetUserOrders(int userid);
    }
}
