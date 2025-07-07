using FuelTrack.Api.Shared.Models;

namespace FuelTrack.Api.Notifications.DTOs;

public class NotificationDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public NotificationType Type { get; set; }
    public bool IsRead { get; set; }
    public int? RelatedOrderId { get; set; }
    public string? RelatedOrderNumber { get; set; }
    public DateTime CreatedAt { get; set; }
}