using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using FuelTrack.Api.Notifications.DTOs;
using FuelTrack.Api.Notifications.Services;

namespace FuelTrack.Api.Notifications.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
[Tags("Notifications")]
public class NotificationsController : ControllerBase
{
    private readonly INotificationService _notificationService;

    public NotificationsController(INotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    /// <summary>
    /// Obtener notificaciones del usuario actual
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<NotificationDto>>> GetNotifications()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (!int.TryParse(userIdClaim, out int userId))
            return Unauthorized();

        var notifications = await _notificationService.GetNotificationsAsync(userId);
        return Ok(notifications);
    }

    /// <summary>
    /// Obtener notificación por ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<NotificationDto>> GetNotification(int id)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (!int.TryParse(userIdClaim, out int userId))
            return Unauthorized();

        var notification = await _notificationService.GetNotificationByIdAsync(id, userId);
        
        if (notification == null)
            return NotFound();
            
        return Ok(notification);
    }

    /// <summary>
    /// Crear nueva notificación (Solo Admin y Proveedor)
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Admin,Proveedor")]
    public async Task<ActionResult<NotificationDto>> CreateNotification([FromBody] CreateNotificationDto createDto)
    {
        var notification = await _notificationService.CreateNotificationAsync(createDto);
        return CreatedAtAction(nameof(GetNotification), new { id = notification.Id }, notification);
    }

    /// <summary>
    /// Marcar notificación como leída
    /// </summary>
    [HttpPatch("{id}/read")]
    public async Task<IActionResult> MarkAsRead(int id)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (!int.TryParse(userIdClaim, out int userId))
            return Unauthorized();

        var result = await _notificationService.MarkAsReadAsync(id, userId);
        
        if (!result)
            return NotFound();
            
        return Ok(new { message = "Notificación marcada como leída" });
    }

    /// <summary>
    /// Marcar todas las notificaciones como leídas
    /// </summary>
    [HttpPatch("read-all")]
    public async Task<IActionResult> MarkAllAsRead()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (!int.TryParse(userIdClaim, out int userId))
            return Unauthorized();

        await _notificationService.MarkAllAsReadAsync(userId);
        return Ok(new { message = "Todas las notificaciones marcadas como leídas" });
    }

    /// <summary>
    /// Obtener cantidad de notificaciones no leídas
    /// </summary>
    [HttpGet("unread-count")]
    public async Task<ActionResult<int>> GetUnreadCount()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (!int.TryParse(userIdClaim, out int userId))
            return Unauthorized();

        var count = await _notificationService.GetUnreadCountAsync(userId);
        return Ok(count);
    }
}