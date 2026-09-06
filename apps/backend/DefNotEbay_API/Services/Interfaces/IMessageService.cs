using DefNotEbay_API.Models;

namespace DefNotEbay_API.Services.Interfaces
{
    public interface IMessageService
    {
        Task<Message> GetMessageAsync(int id);
        Task<bool> CreateMessageAsync(Message message);
        Task<IEnumerable<Message>> GetMessageByConvo(int convoId);
    }
}
