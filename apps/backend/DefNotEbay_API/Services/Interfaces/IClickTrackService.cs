using DefNotEbay_API.Models;

namespace DefNotEbay_API.Services.Interfaces
{
    public interface IClickTrackService
    {
        Task<ClickTrack> RecordClickAsync(int userId, int itemId);

    }
}
