using Microsoft.EntityFrameworkCore;
using FuelTrack.Api.Shared.Data;
using FuelTrack.Api.Shared.Models;
using FuelTrack.Api.Analytics.DTOs;

namespace FuelTrack.Api.Analytics.Services;

public class AnalyticsService : IAnalyticsService
{
    private readonly FuelTrackDbContext _context;

    public AnalyticsService(FuelTrackDbContext context)
    {
        _context = context;
    }

    public async Task<DashboardStatsDto> GetDashboardStatsAsync()
    {
        var totalOrders = await _context.Orders.CountAsync();
        var pendingOrders = await _context.Orders.CountAsync(o => o.Status == OrderStatus.Pending);
        var completedOrders = await _context.Orders.CountAsync(o => o.Status == OrderStatus.Delivered);
        var totalRevenue = await _context.Orders
            .Where(o => o.Status == OrderStatus.Delivered)
            .SumAsync(o => o.TotalAmount);
        var activeVehicles = await _context.Vehicles.CountAsync(v => v.IsActive);
        var availableOperators = await _context.Operators
            .CountAsync(o => o.IsActive && o.Status == OperatorStatus.Available);

        // Monthly revenue for the last 12 months
        var monthlyRevenue = await GetMonthlyRevenueAsync(12);

        // Fuel type statistics
        var fuelTypeStats = await GetFuelTypeStatsAsync();

        return new DashboardStatsDto
        {
            TotalOrders = totalOrders,
            PendingOrders = pendingOrders,
            CompletedOrders = completedOrders,
            TotalRevenue = totalRevenue,
            ActiveVehicles = activeVehicles,
            AvailableOperators = availableOperators,
            MonthlyRevenue = monthlyRevenue,
            FuelTypeStats = fuelTypeStats
        };
    }

    public async Task<UserStatsDto> GetUserStatsAsync()
    {
        var totalUsers = await _context.Users.CountAsync();
        var adminUsers = await _context.Users.CountAsync(u => u.Role == UserRole.Admin);
        var clientUsers = await _context.Users.CountAsync(u => u.Role == UserRole.Cliente);
        var providerUsers = await _context.Users.CountAsync(u => u.Role == UserRole.Proveedor);
        var activeUsers = await _context.Users.CountAsync(u => u.IsActive);

        // Monthly registrations for the last 12 months
        var monthlyRegistrations = await GetMonthlyUserRegistrationsAsync(12);

        return new UserStatsDto
        {
            TotalUsers = totalUsers,
            AdminUsers = adminUsers,
            ClientUsers = clientUsers,
            ProviderUsers = providerUsers,
            ActiveUsers = activeUsers,
            MonthlyRegistrations = monthlyRegistrations
        };
    }

    public async Task<DashboardStatsDto> GetProviderStatsAsync()
{
    try
    {
        Console.WriteLine("📦 Iniciando GetProviderStatsAsync");

        var assignedOrders = await _context.Orders
            .CountAsync(o => o.AssignedVehicleId != null || o.AssignedOperatorId != null);
        Console.WriteLine($"📌 AssignedOrders: {assignedOrders}");

        var inTransitOrders = await _context.Orders
            .CountAsync(o => o.Status == OrderStatus.InTransit);
        Console.WriteLine($"🚚 InTransitOrders: {inTransitOrders}");

        var completedDeliveries = await _context.Orders
            .CountAsync(o => o.Status == OrderStatus.Delivered);
        Console.WriteLine($"✅ CompletedDeliveries: {completedDeliveries}");

        var totalRevenue = await _context.Orders
            .Where(o => o.Status == OrderStatus.Delivered)
            .SumAsync(o => (decimal?)o.TotalAmount) ?? 0;
        Console.WriteLine($"💰 TotalRevenue: {totalRevenue}");

        var activeVehicles = await _context.Vehicles.CountAsync(v => v.IsActive);
        Console.WriteLine($"🚛 ActiveVehicles: {activeVehicles}");

        var availableOperators = await _context.Operators
            .CountAsync(o => o.IsActive && o.Status == OperatorStatus.Available);
        Console.WriteLine($"👷 AvailableOperators: {availableOperators}");

        var monthlyRevenue = await GetMonthlyRevenueAsync(6);
        Console.WriteLine($"📈 MonthlyRevenue count: {monthlyRevenue.Count}");

        Console.WriteLine("✅ Finalizó GetProviderStatsAsync correctamente");

        return new DashboardStatsDto
        {
            TotalOrders = assignedOrders,
            PendingOrders = inTransitOrders,
            CompletedOrders = completedDeliveries,
            TotalRevenue = totalRevenue,
            ActiveVehicles = activeVehicles,
            AvailableOperators = availableOperators,
            MonthlyRevenue = monthlyRevenue,
            FuelTypeStats = new List<FuelTypeStatsDto>()
        };
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ ERROR en GetProviderStatsAsync: {ex}");
        throw;
    }
}

    public async Task<DashboardStatsDto> GetClientStatsAsync(int userId)
    {
        var totalOrders = await _context.Orders.CountAsync(o => o.UserId == userId);
        var pendingOrders = await _context.Orders
            .CountAsync(o => o.UserId == userId && o.Status == OrderStatus.Pending);
        var completedOrders = await _context.Orders
            .CountAsync(o => o.UserId == userId && o.Status == OrderStatus.Delivered);
        var totalSpent = await _context.Orders
            .Where(o => o.UserId == userId && o.Status == OrderStatus.Delivered)
            .SumAsync(o => o.TotalAmount);

        // Monthly spending for the last 6 months
        var monthlySpending = await GetMonthlySpendingAsync(userId, 6);

        // Personal fuel type preferences
        var fuelTypeStats = await GetPersonalFuelTypeStatsAsync(userId);

        return new DashboardStatsDto
        {
            TotalOrders = totalOrders,
            PendingOrders = pendingOrders,
            CompletedOrders = completedOrders,
            TotalRevenue = totalSpent,
            ActiveVehicles = 0,
            AvailableOperators = 0,
            MonthlyRevenue = monthlySpending,
            FuelTypeStats = fuelTypeStats
        };
    }

    private async Task<List<MonthlyRevenueDto>> GetMonthlyRevenueAsync(int months)
    {
        var startDate = DateTime.UtcNow.AddMonths(-months);

        var groupedData = await _context.Orders
            .Where(o => o.CreatedAt >= startDate && o.Status == OrderStatus.Delivered)
            .GroupBy(o => new { o.CreatedAt.Year, o.CreatedAt.Month })
            .ToListAsync();

        var monthlyData = groupedData
            .Select(g => new MonthlyRevenueDto
            {
                Month = $"{g.Key.Year}-{g.Key.Month:D2}",
                Revenue = g.Sum(o => o.TotalAmount),
                OrderCount = g.Count()
            })
            .OrderBy(x => x.Month)
            .ToList();

        return monthlyData;
    }


    private async Task<List<MonthlyRevenueDto>> GetMonthlySpendingAsync(int userId, int months)
    {
        var startDate = DateTime.UtcNow.AddMonths(-months);

        var groupedData = await _context.Orders
            .Where(o => o.UserId == userId && o.CreatedAt >= startDate && o.Status == OrderStatus.Delivered)
            .GroupBy(o => new { o.CreatedAt.Year, o.CreatedAt.Month })
            .ToListAsync(); // Ejecutar en base de datos

        var monthlyData = groupedData
            .Select(g => new MonthlyRevenueDto
            {
                Month = $"{g.Key.Year}-{g.Key.Month:D2}", // Ya en memoria
                Revenue = g.Sum(o => o.TotalAmount),
                OrderCount = g.Count()
            })
            .OrderBy(x => x.Month)
            .ToList();

        return monthlyData;
    }


    private async Task<List<FuelTypeStatsDto>> GetFuelTypeStatsAsync()
    {
        var fuelTypeStats = await _context.Orders
            .Where(o => o.Status == OrderStatus.Delivered)
            .GroupBy(o => o.FuelType)
            .Select(g => new FuelTypeStatsDto
            {
                FuelType = g.Key.ToString(),
                OrderCount = g.Count(),
                TotalQuantity = g.Sum(o => o.Quantity),
                TotalRevenue = g.Sum(o => o.TotalAmount)
            })
            .ToListAsync();

        return fuelTypeStats;
    }

    private async Task<List<FuelTypeStatsDto>> GetPersonalFuelTypeStatsAsync(int userId)
    {
        var fuelTypeStats = await _context.Orders
            .Where(o => o.UserId == userId && o.Status == OrderStatus.Delivered)
            .GroupBy(o => o.FuelType)
            .Select(g => new FuelTypeStatsDto
            {
                FuelType = g.Key.ToString(),
                OrderCount = g.Count(),
                TotalQuantity = g.Sum(o => o.Quantity),
                TotalRevenue = g.Sum(o => o.TotalAmount)
            })
            .ToListAsync();

        return fuelTypeStats;
    }

    private async Task<List<UserRegistrationDto>> GetMonthlyUserRegistrationsAsync(int months)
    {
        var startDate = DateTime.UtcNow.AddMonths(-months);

        var groupedData = await _context.Users
            .Where(u => u.CreatedAt >= startDate)
            .GroupBy(u => new { u.CreatedAt.Year, u.CreatedAt.Month })
            .ToListAsync(); // Ejecuta en la BD y trae a memoria

        var monthlyRegistrations = groupedData
            .Select(g => new UserRegistrationDto
            {
                Month = $"{g.Key.Year}-{g.Key.Month:D2}", // String format en memoria
                Count = g.Count()
            })
            .OrderBy(x => x.Month)
            .ToList();

        return monthlyRegistrations;
    }

}