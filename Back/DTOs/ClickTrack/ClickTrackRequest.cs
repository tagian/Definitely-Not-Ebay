namespace DefNotEbay_API.DTOs.ClickTrack
{
    public class ClickTrackRequest
    {
        public required int UserId { get; set; }
        public required int ItemId { get; set; }
    }
}
