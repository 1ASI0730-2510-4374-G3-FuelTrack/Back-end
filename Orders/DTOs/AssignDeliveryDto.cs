using System.ComponentModel.DataAnnotations;

namespace FuelTrack.Api.Orders.DTOs;

public class AssignDeliveryDto
{
    [Required]
    public int VehicleId { get; set; }
    
    [Required]
    public int OperatorId { get; set; }
    
    public DateTime? EstimatedDeliveryTime { get; set; }
}