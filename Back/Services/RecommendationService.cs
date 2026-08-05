using DefNotEbay_API.Data;
using DefNotEbay_API.Models;
using DefNotEbay_API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;

namespace DefNotEbay_API.Services
{
    public class RecommendationService : IRecommendationService
    {
        private readonly AppDbContext _context;

        public RecommendationService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IReadOnlyList<UserRecommendation>> GetTopRecommendationsForUserAsync(int userId,int top = 30)
        {
            if (top <= 0) top = 30;
            if (top > 100) top = 100;

            var q = _context.UserRecommendations.AsNoTracking().Where(r => r.UserId == userId);


            var best = await q
                .OrderByDescending(r => r.Score)
                .ThenByDescending(r => r.Rank)
                .ThenByDescending(r => r.GeneratedAt)
                .Take(top)
                .ToListAsync();

            ShuffleInPlace(best);
            return best;
        }

        private static void ShuffleInPlace<T>(IList<T> list)
        {
            if (list.Count <= 1) return;
            using var rng = RandomNumberGenerator.Create();

            for (int i = list.Count - 1; i > 0; i--)
            {
                int j = GetRandomInt(rng, i + 1);
                (list[i], list[j]) = (list[j], list[i]);
            }
        }

        private static int GetRandomInt(RandomNumberGenerator rng, int exclusiveUpperBound)
        {
            if (exclusiveUpperBound <= 0)
                throw new ArgumentOutOfRangeException(nameof(exclusiveUpperBound));

            Span<byte> b = stackalloc byte[4];
            uint limit = (uint.MaxValue / (uint)exclusiveUpperBound) * (uint)exclusiveUpperBound;

            uint r;
            do
            {
                rng.GetBytes(b);
                r = BitConverter.ToUInt32(b);
            } while (r >= limit);

            return (int)(r % (uint)exclusiveUpperBound);
        }
    }

}
