using System.ComponentModel.DataAnnotations;

namespace FuelTrack.Api.Payments.DTOs;

public class CreatePaymentMethodDto
{
    [Required]
    [MaxLength(100)]
    public string CardHolderName { get; set; } = string.Empty;
    
    [Required]
    [CreditCard]
    public string CardNumber { get; set; } = string.Empty;
    
    [Required]
    [Range(1, 12)]
    public int ExpiryMonth { get; set; }
    
    [Required]
    [Range(2024, 2050)]
    public int ExpiryYear { get; set; }
    
    [Required]
    [StringLength(4, MinimumLength = 3)]
    public string CVV { get; set; } = string.Empty;
    
    public bool IsDefault { get; set; } = false;
}