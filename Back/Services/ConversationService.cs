using DefNotEbay_API.Data;
using DefNotEbay_API.Models;
using DefNotEbay_API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DefNotEbay_API.Services
{
    public class ConversationService : IConversationService
    {
        private readonly AppDbContext _context;
        public ConversationService(AppDbContext context)
        {
            _context = context;
        }
        public async Task<bool> CreateConversationAsync(Conversation conversation)
        {
            conversation.CreatedAt = DateTime.UtcNow;
            _context.Conversations.Add(conversation);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteConversationAsync(int id)
        {
            var conversation = await _context.Conversations.FindAsync(id);
            if (conversation == null)
            {
                return false; 
            }
            _context.Conversations.Remove(conversation);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<Conversation?> GetConversationAsync(int id)
        {
           var conversation = await _context.Conversations.FindAsync(id);
            if (conversation == null)
            {
                return null;
            }
            return conversation;
        }

        public async Task<IEnumerable<Conversation?>> GetUserConversations(int userid)
        {
            var Conversations = await _context.Conversations.Where(c => c.UserAId == userid || c.UserBId == userid).ToListAsync();

            return Conversations;
        }
    }
}
