using FuelTrack.Api.Analytics.DTOs;

namespace FuelTrack.Api.Analytics.Services;

public interface IAnalyticsService
{
    Task<DashboardStatsDto> GetDashboardStatsAsync();
    Task<UserStatsDto> GetUserStatsAsync();
    Task<DashboardStatsDto> GetProviderStatsAsync();
    Task<DashboardStatsDto> GetClientStatsAsync(int userId);
}