using System.ComponentModel.DataAnnotations;
using FuelTrack.Api.Shared.Models;

namespace FuelTrack.Api.Orders.DTOs;

public class CreateOrderDto
{
    [Required]
    public FuelType FuelType { get; set; }
    
    [Required]
    [Range(1, 50000)]
    public decimal Quantity { get; set; }
    
    [Required]
    [MaxLength(500)]
    public string DeliveryAddress { get; set; } = string.Empty;
    
    public double? DeliveryLatitude { get; set; }
    public double? DeliveryLongitude { get; set; }
}