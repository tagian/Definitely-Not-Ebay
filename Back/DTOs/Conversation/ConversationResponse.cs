namespace DefNotEbay_API.DTOs.Conversation
{
    public class ConversationResponse
    {
        public int ConversationId { get; set; }
        public int UserAId { get; set; }
        public int UserBId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
