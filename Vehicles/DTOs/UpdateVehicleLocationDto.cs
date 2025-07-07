using System.ComponentModel.DataAnnotations;

namespace FuelTrack.Api.Vehicles.DTOs;

public class UpdateVehicleLocationDto
{
    [Required]
    [Range(-90, 90)]
    public double Latitude { get; set; }
    
    [Required]
    [Range(-180, 180)]
    public double Longitude { get; set; }
}