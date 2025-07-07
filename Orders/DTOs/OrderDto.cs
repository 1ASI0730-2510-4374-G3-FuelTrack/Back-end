using FuelTrack.Api.Shared.Models;

namespace FuelTrack.Api.Orders.DTOs;

public class OrderDto
{
    public int Id { get; set; }
    public string OrderNumber { get; set; } = string.Empty;
    public FuelType FuelType { get; set; }
    public decimal Quantity { get; set; }
    public decimal PricePerLiter { get; set; }
    public decimal TotalAmount { get; set; }
    public OrderStatus Status { get; set; }
    public string DeliveryAddress { get; set; } = string.Empty;
    public double? DeliveryLatitude { get; set; }
    public double? DeliveryLongitude { get; set; }
    public DateTime? EstimatedDeliveryTime { get; set; }
    public DateTime? ActualDeliveryTime { get; set; }
    public DateTime CreatedAt { get; set; }
    
    // Related entities
    public string CustomerName { get; set; } = string.Empty;
    public string? AssignedVehiclePlate { get; set; }
    public string? AssignedOperatorName { get; set; }
}