using FuelTrack.Api.Shared.Models;

namespace FuelTrack.Api.Vehicles.DTOs;

public class VehicleDto
{
    public int Id { get; set; }
    public string LicensePlate { get; set; } = string.Empty;
    public string Brand { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public int Year { get; set; }
    public decimal Capacity { get; set; }
    public VehicleStatus Status { get; set; }
    public double? CurrentLatitude { get; set; }
    public double? CurrentLongitude { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
}