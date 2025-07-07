using FuelTrack.Api.Shared.Models;
using Microsoft.EntityFrameworkCore;

namespace FuelTrack.Api.Shared.Data;

public static class SeedData
{
    public static async Task Initialize(FuelTrackDbContext context)
    {
        // Check if data already exists
        if (await context.Users.AnyAsync())
            return;

        // Create admin user
        var adminUser = new User
        {
            FirstName = "Admin",
            LastName = "System",
            Email = "admin@fueltrack.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin123!"),
            Role = UserRole.Admin,
            Phone = "+1234567890"
        };

        // Create sample client
        var clientUser = new User
        {
            FirstName = "Juan",
            LastName = "Pérez",
            Email = "cliente@fueltrack.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Cliente123!"),
            Role = UserRole.Cliente,
            Phone = "+1234567891"
        };

        // Create sample provider
        var providerUser = new User
        {
            FirstName = "María",
            LastName = "García",
            Email = "proveedor@fueltrack.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Proveedor123!"),
            Role = UserRole.Proveedor,
            Phone = "+1234567892"
        };

        context.Users.AddRange(adminUser, clientUser, providerUser);

        // Create sample vehicles
        var vehicles = new List<Vehicle>
        {
            new Vehicle
            {
                LicensePlate = "ABC-123",
                Brand = "Mercedes",
                Model = "Actros",
                Year = 2022,
                Capacity = 10000,
                Status = VehicleStatus.Available
            },
            new Vehicle
            {
                LicensePlate = "DEF-456",
                Brand = "Volvo",
                Model = "FH",
                Year = 2021,
                Capacity = 15000,
                Status = VehicleStatus.Available
            }
        };

        context.Vehicles.AddRange(vehicles);

        // Create sample operators
        var operators = new List<Operator>
        {
            new Operator
            {
                FirstName = "Carlos",
                LastName = "Rodríguez",
                LicenseNumber = "LIC123456",
                LicenseExpiryDate = DateTime.UtcNow.AddYears(2),
                Phone = "+1234567893",
                Status = OperatorStatus.Available
            },
            new Operator
            {
                FirstName = "Ana",
                LastName = "López",
                LicenseNumber = "LIC789012",
                LicenseExpiryDate = DateTime.UtcNow.AddYears(3),
                Phone = "+1234567894",
                Status = OperatorStatus.Available
            }
        };

        context.Operators.AddRange(operators);

        await context.SaveChangesAsync();
    }
}