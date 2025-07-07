using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using FuelTrack.Api.Analytics.DTOs;
using FuelTrack.Api.Analytics.Services;

namespace FuelTrack.Api.Analytics.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
[Tags("Analytics")]
public class AnalyticsController : ControllerBase
{
    private readonly IAnalyticsService _analyticsService;

    public AnalyticsController(IAnalyticsService analyticsService)
    {
        _analyticsService = analyticsService;
    }

    /// <summary>
    /// Obtener estadísticas del dashboard (Solo Admin)
    /// </summary>
    [HttpGet("dashboard")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<DashboardStatsDto>> GetDashboardStats()
    {
        var stats = await _analyticsService.GetDashboardStatsAsync();
        return Ok(stats);
    }

    /// <summary>
    /// Obtener estadísticas de usuarios (Solo Admin)
    /// </summary>
    [HttpGet("users")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<UserStatsDto>> GetUserStats()
    {
        var stats = await _analyticsService.GetUserStatsAsync();
        return Ok(stats);
    }

    /// <summary>
    /// Obtener estadísticas del proveedor (Solo Proveedor)
    /// </summary>
    [HttpGet("provider")]
    [Authorize(Roles = "Proveedor")]
    public async Task<ActionResult<DashboardStatsDto>> GetProviderStats()
    {
        var stats = await _analyticsService.GetProviderStatsAsync();
        return Ok(stats);
    }

    /// <summary>
    /// Obtener estadísticas del cliente (Solo Cliente)
    /// </summary>
    [HttpGet("client")]
    [Authorize(Roles = "Cliente")]
    public async Task<ActionResult<DashboardStatsDto>> GetClientStats()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (!int.TryParse(userIdClaim, out int userId))
            return Unauthorized();

        var stats = await _analyticsService.GetClientStatsAsync(userId);
        return Ok(stats);
    }
}