using System.ComponentModel.DataAnnotations;

namespace FuelTrack.Api.Operators.DTOs;

public class CreateOperatorDto
{
    [Required]
    [MaxLength(100)]
    public string FirstName { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(100)]
    public string LastName { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(20)]
    public string LicenseNumber { get; set; } = string.Empty;
    
    [Required]
    public DateTime LicenseExpiryDate { get; set; }
    
    [MaxLength(20)]
    public string? Phone { get; set; }
}