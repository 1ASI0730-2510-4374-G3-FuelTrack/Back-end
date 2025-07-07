using System.Security.Claims;

namespace FuelTrack.Api.Shared.Middleware;

public class AccessLogMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<AccessLogMiddleware> _logger;

    public AccessLogMiddleware(RequestDelegate next, ILogger<AccessLogMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var startTime = DateTime.UtcNow;
        
        await _next(context);
        
        var endTime = DateTime.UtcNow;
        var duration = endTime - startTime;
        
        var userEmail = context.User?.FindFirst(ClaimTypes.Email)?.Value ?? "Anonymous";
        var userRole = context.User?.FindFirst(ClaimTypes.Role)?.Value ?? "None";
        
        _logger.LogInformation(
            "Access Log - User: {UserEmail}, Role: {UserRole}, Method: {Method}, Path: {Path}, StatusCode: {StatusCode}, Duration: {Duration}ms",
            userEmail, userRole, context.Request.Method, context.Request.Path, 
            context.Response.StatusCode, duration.TotalMilliseconds);
    }
}