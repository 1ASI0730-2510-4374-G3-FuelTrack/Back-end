namespace FuelTrack.Api.Payments.DTOs;

public class PaymentMethodDto
{
    public int Id { get; set; }
    public string CardHolderName { get; set; } = string.Empty;
    public string LastFourDigits { get; set; } = string.Empty;
    public string CardType { get; set; } = string.Empty;
    public DateTime ExpiryDate { get; set; }
    public bool IsDefault { get; set; }
    public DateTime CreatedAt { get; set; }
}