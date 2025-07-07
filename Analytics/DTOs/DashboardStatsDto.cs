namespace FuelTrack.Api.Analytics.DTOs;

public class DashboardStatsDto
{
    public int TotalOrders { get; set; }
    public int PendingOrders { get; set; }
    public int CompletedOrders { get; set; }
    public decimal TotalRevenue { get; set; }
    public int ActiveVehicles { get; set; }
    public int AvailableOperators { get; set; }
    public List<MonthlyRevenueDto> MonthlyRevenue { get; set; } = new();
    public List<FuelTypeStatsDto> FuelTypeStats { get; set; } = new();
}

public class MonthlyRevenueDto
{
    public string Month { get; set; } = string.Empty;
    public decimal Revenue { get; set; }
    public int OrderCount { get; set; }
}

public class FuelTypeStatsDto
{
    public string FuelType { get; set; } = string.Empty;
    public int OrderCount { get; set; }
    public decimal TotalQuantity { get; set; }
    public decimal TotalRevenue { get; set; }
}