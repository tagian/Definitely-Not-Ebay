namespace DefNotEbay_API.DTOs.Message
{
    public class CreateMessageRequest
    {
        public required int SenderId { get; set; }
        public required int ReceipientId { get; set; }
        public required string Content { get; set; }
        public required DateTime SentAt { get; set; }
        public required int ConversationId { get; set; }
    }
}
