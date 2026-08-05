namespace DefNotEbay_API.Models
{
    public class ClickTrack
    {
        public long ClickTrackId { get; set; }
        public required int UserId { get; set; }
        public required int ItemId { get; set; }       
        public long Clicks { get; set; } = 0;
        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
        public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;

    }
}
