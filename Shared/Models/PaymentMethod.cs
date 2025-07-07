using System.ComponentModel.DataAnnotations;

namespace FuelTrack.Api.Shared.Models;

public class PaymentMethod : BaseEntity
{
    [Required]
    public int UserId { get; set; }
    
    [Required]
    [MaxLength(100)]
    public string CardHolderName { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(4)]
    public string LastFourDigits { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(50)]
    public string CardType { get; set; } = string.Empty;
    
    [Required]
    public string EncryptedCardNumber { get; set; } = string.Empty;
    
    public DateTime ExpiryDate { get; set; }
    
    public bool IsDefault { get; set; } = false;
    
    // Navigation properties
    public virtual User User { get; set; } = null!;
    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();
}