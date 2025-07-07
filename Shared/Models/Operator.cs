using System.ComponentModel.DataAnnotations;

namespace FuelTrack.Api.Shared.Models;

public class Operator : BaseEntity
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
    
    public DateTime LicenseExpiryDate { get; set; }
    
    [MaxLength(20)]
    public string? Phone { get; set; }
    
    public OperatorStatus Status { get; set; } = OperatorStatus.Available;
    
    // Navigation properties
    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}

public enum OperatorStatus
{
    Available = 1,
    OnDelivery = 2,
    OffDuty = 3
}