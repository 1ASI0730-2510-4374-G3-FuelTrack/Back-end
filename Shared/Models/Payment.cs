using System.ComponentModel.DataAnnotations;

namespace FuelTrack.Api.Shared.Models;

public class Payment : BaseEntity
{
    [Required]
    public int OrderId { get; set; }
    
    [Required]
    public int PaymentMethodId { get; set; }
    
    [Required]
    public decimal Amount { get; set; }
    
    public PaymentStatus Status { get; set; } = PaymentStatus.Pending;
    
    [MaxLength(100)]
    public string? TransactionId { get; set; }
    
    public DateTime? ProcessedAt { get; set; }
    
    // Navigation properties
    public virtual Order Order { get; set; } = null!;
    public virtual PaymentMethod PaymentMethod { get; set; } = null!;
}

public enum PaymentStatus
{
    Pending = 1,
    Completed = 2,
    Failed = 3,
    Refunded = 4
}