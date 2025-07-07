using FuelTrack.Api.Shared.Models;

namespace FuelTrack.Api.Payments.DTOs;

public class PaymentDto
{
    public int Id { get; set; }
    public int OrderId { get; set; }
    public string OrderNumber { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public PaymentStatus Status { get; set; }
    public string? TransactionId { get; set; }
    public DateTime? ProcessedAt { get; set; }
    public DateTime CreatedAt { get; set; }
    
    // Payment method info
    public string CardHolderName { get; set; } = string.Empty;
    public string LastFourDigits { get; set; } = string.Empty;
    public string CardType { get; set; } = string.Empty;
}