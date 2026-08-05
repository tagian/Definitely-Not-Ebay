using DefNotEbay_API.DTOs.AdminStats;

namespace DefNotEbay_API.Services.Interfaces
{
    public interface IAdminStatsService
    {
        Task<AdminStatsResponse> GetAdminStatsAsync();

    }
}
