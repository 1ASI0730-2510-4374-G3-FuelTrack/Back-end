using Microsoft.EntityFrameworkCore;
using FuelTrack.Api.Shared.Models;

namespace FuelTrack.Api.Shared.Data;

public class FuelTrackDbContext : DbContext
{
    public FuelTrackDbContext(DbContextOptions<FuelTrackDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<Payment> Payments { get; set; }
    public DbSet<PaymentMethod> PaymentMethods { get; set; }
    public DbSet<Vehicle> Vehicles { get; set; }
    public DbSet<Operator> Operators { get; set; }
    public DbSet<Notification> Notifications { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // User configuration
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasIndex(e => e.Email).IsUnique();
            entity.Property(e => e.Role).HasConversion<int>();
        });

        // Order configuration
        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasIndex(e => e.OrderNumber).IsUnique();
            entity.Property(e => e.FuelType).HasConversion<int>();
            entity.Property(e => e.Status).HasConversion<int>();
            entity.Property(e => e.TotalAmount).HasPrecision(18, 2);
            entity.Property(e => e.PricePerLiter).HasPrecision(18, 2);
            entity.Property(e => e.Quantity).HasPrecision(18, 2);
            
            entity.HasOne(e => e.User)
                  .WithMany(e => e.Orders)
                  .HasForeignKey(e => e.UserId)
                  .OnDelete(DeleteBehavior.Restrict);
                  
            entity.HasOne(e => e.AssignedVehicle)
                  .WithMany(e => e.Orders)
                  .HasForeignKey(e => e.AssignedVehicleId)
                  .OnDelete(DeleteBehavior.SetNull);
                  
            entity.HasOne(e => e.AssignedOperator)
                  .WithMany(e => e.Orders)
                  .HasForeignKey(e => e.AssignedOperatorId)
                  .OnDelete(DeleteBehavior.SetNull);
        });

        // Payment configuration
        modelBuilder.Entity<Payment>(entity =>
        {
            entity.Property(e => e.Status).HasConversion<int>();
            entity.Property(e => e.Amount).HasPrecision(18, 2);
            
            entity.HasOne(e => e.Order)
                  .WithMany(e => e.Payments)
                  .HasForeignKey(e => e.OrderId)
                  .OnDelete(DeleteBehavior.Cascade);
                  
            entity.HasOne(e => e.PaymentMethod)
                  .WithMany(e => e.Payments)
                  .HasForeignKey(e => e.PaymentMethodId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // PaymentMethod configuration
        modelBuilder.Entity<PaymentMethod>(entity =>
        {
            entity.HasOne(e => e.User)
                  .WithMany(e => e.PaymentMethods)
                  .HasForeignKey(e => e.UserId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // Vehicle configuration
        modelBuilder.Entity<Vehicle>(entity =>
        {
            entity.HasIndex(e => e.LicensePlate).IsUnique();
            entity.Property(e => e.Status).HasConversion<int>();
            entity.Property(e => e.Capacity).HasPrecision(18, 2);
        });

        // Operator configuration
        modelBuilder.Entity<Operator>(entity =>
        {
            entity.HasIndex(e => e.LicenseNumber).IsUnique();
            entity.Property(e => e.Status).HasConversion<int>();
        });

        // Notification configuration
        modelBuilder.Entity<Notification>(entity =>
        {
            entity.Property(e => e.Type).HasConversion<int>();
            
            entity.HasOne(e => e.User)
                  .WithMany(e => e.Notifications)
                  .HasForeignKey(e => e.UserId)
                  .OnDelete(DeleteBehavior.Cascade);
                  
            entity.HasOne(e => e.RelatedOrder)
                  .WithMany()
                  .HasForeignKey(e => e.RelatedOrderId)
                  .OnDelete(DeleteBehavior.SetNull);
        });
    }
}