using System.ComponentModel.DataAnnotations;

namespace FuelTrack.Api.Shared.Models;

public class Order : BaseEntity
{
    [Required]
    public int UserId { get; set; }
    
    [Required]
    [MaxLength(50)]
    public string OrderNumber { get; set; } = string.Empty;
    
    public FuelType FuelType { get; set; }
    
    [Required]
    public decimal Quantity { get; set; }
    
    [Required]
    public decimal PricePerLiter { get; set; }
    
    public decimal TotalAmount { get; set; }
    
    public OrderStatus Status { get; set; } = OrderStatus.Pending;
    
    [MaxLength(500)]
    public string DeliveryAddress { get; set; } = string.Empty;
    
    public double? DeliveryLatitude { get; set; }
    public double? DeliveryLongitude { get; set; }
    
    public DateTime? EstimatedDeliveryTime { get; set; }
    public DateTime? ActualDeliveryTime { get; set; }
    
    public int? AssignedVehicleId { get; set; }
    public int? AssignedOperatorId { get; set; }
    
    // Navigation properties
    public virtual User User { get; set; } = null!;
    public virtual Vehicle? AssignedVehicle { get; set; }
    public virtual Operator? AssignedOperator { get; set; }
    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();
}

public enum FuelType
{
    Gasoline = 1,
    Diesel = 2,
    Premium = 3
}

public enum OrderStatus
{
    Pending = 1,
    Confirmed = 2,
    InTransit = 3,
    Delivered = 4,
    Cancelled = 5
}