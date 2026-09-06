using DefNotEbay_API.Data;
using DefNotEbay_API.Models;
using DefNotEbay_API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DefNotEbay_API.Services
{
    public class MessageService : IMessageService
    {
        private readonly AppDbContext _context;
        public MessageService(AppDbContext context)
        {
            _context = context;
        }
        public async Task<bool> CreateMessageAsync(Message message)
        {
            await _context.Messages.AddAsync(message);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<Message> GetMessageAsync(int id)
        {
            var message = await _context.Messages.FirstOrDefaultAsync(m => m.MessageId == id);
            return message ?? throw new KeyNotFoundException($"Message with ID {id} not found.");
        }

        public async Task<IEnumerable<Message>> GetMessageByConvo(int convoId)
        {
            var messages = await _context.Messages.Where(m => (m.ConversationId == convoId)).OrderBy(m => m.SentAt).ToListAsync();
            return messages;
        }
    }
}
