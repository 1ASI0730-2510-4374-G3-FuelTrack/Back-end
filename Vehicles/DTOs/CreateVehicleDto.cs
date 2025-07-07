using System.ComponentModel.DataAnnotations;

namespace FuelTrack.Api.Vehicles.DTOs;

public class CreateVehicleDto
{
    [Required]
    [MaxLength(20)]
    public string LicensePlate { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(100)]
    public string Brand { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(100)]
    public string Model { get; set; } = string.Empty;
    
    [Required]
    [Range(1990, 2030)]
    public int Year { get; set; }
    
    [Required]
    [Range(1000, 50000)]
    public decimal Capacity { get; set; }
}