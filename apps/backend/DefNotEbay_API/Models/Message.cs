namespace DefNotEbay_API.Models
{
    public class Message
    {
        public int MessageId {  get; set; }
        public required int SenderId { get; set; }
        public required int ReceipientId { get; set; }
        public required string Content {  get; set; }
        public required DateTime SentAt { get; set; }
        public required bool IsRead { get; set; }
        public DateTime? ReadAt { get; set; }
        public required int ConversationId {  get; set; }
        public required Conversation Conversation { get; set; }

    }
}
