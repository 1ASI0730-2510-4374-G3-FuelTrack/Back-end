using System.ComponentModel.DataAnnotations;

namespace FuelTrack.Api.Shared.Models;

public class Notification : BaseEntity
{
    [Required]
    public int UserId { get; set; }
    
    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(1000)]
    public string Message { get; set; } = string.Empty;
    
    public NotificationType Type { get; set; }
    
    public bool IsRead { get; set; } = false;
    
    public int? RelatedOrderId { get; set; }
    
    // Navigation properties
    public virtual User User { get; set; } = null!;
    public virtual Order? RelatedOrder { get; set; }
}

public enum NotificationType
{
    OrderUpdate = 1,
    PaymentConfirmation = 2,
    DeliveryAlert = 3,
    SystemNotification = 4
}