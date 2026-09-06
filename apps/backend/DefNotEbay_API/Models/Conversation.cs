namespace DefNotEbay_API.Models
{
    public class Conversation
    {
        public int ConversationId { get; set; }
        public required int UserAId { get; set; }
        public required int UserBId { get; set; }
        public required ICollection<Message> Messages { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
