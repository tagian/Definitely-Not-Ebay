using DefNotEbay_API.Models;

namespace DefNotEbay_API.Services.Interfaces
{
    public interface IConversationService
    {
        Task<Conversation?> GetConversationAsync(int id);
        Task<bool> CreateConversationAsync(Conversation conversation);
        Task<bool> DeleteConversationAsync(int id);
        Task<IEnumerable<Conversation?>> GetUserConversations(int userid);
    }
}
