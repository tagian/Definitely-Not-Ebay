namespace DefNotEbay_API.DTOs.Message
{
    public class MessageResponse
    {
        public int MessageId { get; set; }
        public  int SenderId { get; set; }
        public  int ReceipientId { get; set; }
        public string? Content { get; set; }
        public DateTime SentAt { get; set; }
        public bool IsRead { get; set; }
        public DateTime ReadAt { get; set; }
        public int ConversationId { get; set; }
    }
}
