using DefNotEbay_API.Data;
using DefNotEbay_API.Models;
using DefNotEbay_API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DefNotEbay_API.Services
{
    public class TrackService : IClickTrackService
    {
        private readonly AppDbContext _context;

        public TrackService(AppDbContext context)
        {
            _context = context;
        }
        public async Task<ClickTrack> RecordClickAsync(int userId, int itemId)
        {
            var track = await _context.ClickTracks.SingleOrDefaultAsync(x => x.UserId == userId && x.ItemId == itemId);

            if (track is null)
            {
                track = new ClickTrack
                {
                    UserId = userId,
                    ItemId = itemId,
                    Clicks = 1
                };

                _context.ClickTracks.Add(track);
            }
            else
            {
                track.Clicks += 1;
                _context.ClickTracks.Update(track);
            }

            var saved = false;
            while (!saved)
            {
                try
                {
                    await _context.SaveChangesAsync();
                    saved = true;
                }
                catch (DbUpdateConcurrencyException)
                {
                    var entry = _context.Entry(track);
                    await entry.ReloadAsync();
                    track.Clicks += 1; 
                }
            }

            return track;
        }

    }
}

