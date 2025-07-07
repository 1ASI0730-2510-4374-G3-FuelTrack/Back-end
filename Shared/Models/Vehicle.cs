using System.ComponentModel.DataAnnotations;

namespace FuelTrack.Api.Shared.Models;

public class Vehicle : BaseEntity
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
    
    public int Year { get; set; }
    
    [Required]
    public decimal Capacity { get; set; }
    
    public VehicleStatus Status { get; set; } = VehicleStatus.Available;
    
    public double? CurrentLatitude { get; set; }
    public double? CurrentLongitude { get; set; }
    
    // Navigation properties
    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}

public enum VehicleStatus
{
    Available = 1,
    InUse = 2,
    Maintenance = 3,
    OutOfService = 4
}