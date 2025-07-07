namespace FuelTrack.Api.Analytics.DTOs;

public class UserStatsDto
{
    public int TotalUsers { get; set; }
    public int AdminUsers { get; set; }
    public int ClientUsers { get; set; }
    public int ProviderUsers { get; set; }
    public int ActiveUsers { get; set; }
    public List<UserRegistrationDto> MonthlyRegistrations { get; set; } = new();
}

public class UserRegistrationDto
{
    public string Month { get; set; } = string.Empty;
    public int Count { get; set; }
}