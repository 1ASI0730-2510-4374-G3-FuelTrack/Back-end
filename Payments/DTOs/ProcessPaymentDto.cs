using System.ComponentModel.DataAnnotations;

namespace FuelTrack.Api.Payments.DTOs;

public class ProcessPaymentDto
{
    [Required]
    public int OrderId { get; set; }
    
    [Required]
    public int PaymentMethodId { get; set; }
}