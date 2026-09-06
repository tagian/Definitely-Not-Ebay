using DefNotEbay_API.Data;
using DefNotEbay_API.Models;
using DefNotEbay_API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Asn1.X509;

namespace DefNotEbay_API.Services
{
    public class OrderService : IOrderService
    {
        private readonly AppDbContext _context;
        private readonly IConversationService _conversations;

        public OrderService(AppDbContext context, IConversationService conversations)
        {
            _context = context;
            _conversations = conversations;
        }
        public async Task<bool> CreateOrder(Order order)
        {
            order.DateCreated = DateTime.UtcNow;
            _context.Orders.Add(order);
            
            var newConvo = new Conversation
            {
                UserAId = order.SellerId,
                UserBId = order.BuyerId,
                Messages = new List<Message>(),
                CreatedAt = DateTime.UtcNow
            };

            var entry = _context.Conversations.Add(newConvo);
            await _context.SaveChangesAsync();
            var convoId = entry.Entity.ConversationId;
            var adminId = await _context.Users.Where(u => u.Role == "Admin").Select(u => u.UserId).FirstOrDefaultAsync();
            var itemName = await _context.Items.Where(i => i.ItemId == order.ItemId).Select(i => i.Name).FirstOrDefaultAsync();
            var newMessage = new Message
            {
                SenderId = adminId,
                ReceipientId = order.SellerId,
                Content = $"Order of {itemName} Completed successfully. Talk About Minor Details (Payment, Shipping etc)",
                SentAt = DateTime.UtcNow,
                IsRead = false,
                ConversationId = convoId,
                Conversation = newConvo
            };
            _context.Messages.Add(newMessage);

            newMessage = new Message
            {
                SenderId = adminId,
                ReceipientId = order.BuyerId,
                Content = $"Order of {itemName} Completed successfully. Talk About Minor Details (Payment, Shipping etc)",
                SentAt = DateTime.UtcNow,
                IsRead = false,
                ConversationId = convoId,
            Conversation = newConvo
            };
            _context.Messages.Add(newMessage);


            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<Order> GetOrder(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                throw new KeyNotFoundException($"Order with ID {id} not found.");
            }
            return order;
        }

        async Task<IEnumerable<Order>> IOrderService.GetAllOrders()
        {
            var orders = await _context.Orders.ToListAsync();
            if (orders == null)
            {
                throw new KeyNotFoundException($"No orders not found.");
            }
            return orders;
        }

        async Task<IEnumerable<Order>> IOrderService.GetUserOrders(int userid)
        {
            var orders = await _context.Orders.Where(o => o.BuyerId == userid || o.SellerId == userid).ToListAsync();
            return orders;

        }
    }
}
