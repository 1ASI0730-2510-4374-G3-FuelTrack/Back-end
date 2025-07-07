using System.ComponentModel.DataAnnotations;
using FuelTrack.Api.Shared.Models;

namespace FuelTrack.Api.Notifications.DTOs;

public class CreateNotificationDto
{
    [Required]
    public int UserId { get; set; }
    
    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(1000)]
    public string Message { get; set; } = string.Empty;
    
    [Required]
    public NotificationType Type { get; set; }
    
    public int? RelatedOrderId { get; set; }
}