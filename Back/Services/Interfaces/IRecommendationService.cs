using DefNotEbay_API.Models;

namespace DefNotEbay_API.Services.Interfaces
{
    public interface IRecommendationService
    {
        Task<IReadOnlyList<UserRecommendation>> GetTopRecommendationsForUserAsync(int userId, int top = 30);
    }

}
