using Microsoft.EntityFrameworkCore;
using FuelTrack.Api.Shared.Data;
using FuelTrack.Api.Shared.Models;
using FuelTrack.Api.Vehicles.DTOs;

namespace FuelTrack.Api.Vehicles.Services;

public class VehicleService : IVehicleService
{
    private readonly FuelTrackDbContext _context;

    public VehicleService(FuelTrackDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<VehicleDto>> GetVehiclesAsync()
    {
        return await _context.Vehicles
            .Where(v => v.IsActive)
            .Select(v => new VehicleDto
            {
                Id = v.Id,
                LicensePlate = v.LicensePlate,
                Brand = v.Brand,
                Model = v.Model,
                Year = v.Year,
                Capacity = v.Capacity,
                Status = v.Status,
                CurrentLatitude = v.CurrentLatitude,
                CurrentLongitude = v.CurrentLongitude,
                IsActive = v.IsActive,
                CreatedAt = v.CreatedAt
            })
            .OrderBy(v => v.LicensePlate)
            .ToListAsync();
    }

    public async Task<VehicleDto?> GetVehicleByIdAsync(int id)
    {
        var vehicle = await _context.Vehicles
            .Where(v => v.Id == id && v.IsActive)
            .FirstOrDefaultAsync();

        if (vehicle == null)
            return null;

        return new VehicleDto
        {
            Id = vehicle.Id,
            LicensePlate = vehicle.LicensePlate,
            Brand = vehicle.Brand,
            Model = vehicle.Model,
            Year = vehicle.Year,
            Capacity = vehicle.Capacity,
            Status = vehicle.Status,
            CurrentLatitude = vehicle.CurrentLatitude,
            CurrentLongitude = vehicle.CurrentLongitude,
            IsActive = vehicle.IsActive,
            CreatedAt = vehicle.CreatedAt
        };
    }

    public async Task<VehicleDto> CreateVehicleAsync(CreateVehicleDto createDto)
    {
        if (await _context.Vehicles.AnyAsync(v => v.LicensePlate == createDto.LicensePlate && v.IsActive))
        {
            throw new InvalidOperationException("Ya existe un vehículo con esta placa");
        }

        var vehicle = new Vehicle
        {
            LicensePlate = createDto.LicensePlate,
            Brand = createDto.Brand,
            Model = createDto.Model,
            Year = createDto.Year,
            Capacity = createDto.Capacity,
            Status = VehicleStatus.Available
        };

        _context.Vehicles.Add(vehicle);
        await _context.SaveChangesAsync();

        return new VehicleDto
        {
            Id = vehicle.Id,
            LicensePlate = vehicle.LicensePlate,
            Brand = vehicle.Brand,
            Model = vehicle.Model,
            Year = vehicle.Year,
            Capacity = vehicle.Capacity,
            Status = vehicle.Status,
            CurrentLatitude = vehicle.CurrentLatitude,
            CurrentLongitude = vehicle.CurrentLongitude,
            IsActive = vehicle.IsActive,
            CreatedAt = vehicle.CreatedAt
        };
    }

    public async Task<VehicleDto> UpdateVehicleAsync(int id, CreateVehicleDto updateDto)
    {
        var vehicle = await _context.Vehicles.FindAsync(id);

        if (vehicle == null || !vehicle.IsActive)
            throw new KeyNotFoundException("Vehículo no encontrado");

        if (await _context.Vehicles.AnyAsync(v => v.LicensePlate == updateDto.LicensePlate && v.Id != id && v.IsActive))
        {
            throw new InvalidOperationException("Ya existe un vehículo con esta placa");
        }

        vehicle.LicensePlate = updateDto.LicensePlate;
        vehicle.Brand = updateDto.Brand;
        vehicle.Model = updateDto.Model;
        vehicle.Year = updateDto.Year;
        vehicle.Capacity = updateDto.Capacity;
        vehicle.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return new VehicleDto
        {
            Id = vehicle.Id,
            LicensePlate = vehicle.LicensePlate,
            Brand = vehicle.Brand,
            Model = vehicle.Model,
            Year = vehicle.Year,
            Capacity = vehicle.Capacity,
            Status = vehicle.Status,
            CurrentLatitude = vehicle.CurrentLatitude,
            CurrentLongitude = vehicle.CurrentLongitude,
            IsActive = vehicle.IsActive,
            CreatedAt = vehicle.CreatedAt
        };
    }

    public async Task<bool> DeleteVehicleAsync(int id)
    {
        var vehicle = await _context.Vehicles.FindAsync(id);

        if (vehicle == null || !vehicle.IsActive)
            return false;

        // Check if vehicle is currently assigned to any active order
        var hasActiveOrders = await _context.Orders
            .AnyAsync(o => o.AssignedVehicleId == id && 
                          (o.Status == OrderStatus.Confirmed || o.Status == OrderStatus.InTransit));

        if (hasActiveOrders)
            throw new InvalidOperationException("No se puede eliminar un vehículo con pedidos activos");

        vehicle.IsActive = false;
        vehicle.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<VehicleDto> UpdateVehicleLocationAsync(int id, UpdateVehicleLocationDto locationDto)
    {
        var vehicle = await _context.Vehicles.FindAsync(id);

        if (vehicle == null || !vehicle.IsActive)
            throw new KeyNotFoundException("Vehículo no encontrado");

        vehicle.CurrentLatitude = locationDto.Latitude;
        vehicle.CurrentLongitude = locationDto.Longitude;
        vehicle.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return new VehicleDto
        {
            Id = vehicle.Id,
            LicensePlate = vehicle.LicensePlate,
            Brand = vehicle.Brand,
            Model = vehicle.Model,
            Year = vehicle.Year,
            Capacity = vehicle.Capacity,
            Status = vehicle.Status,
            CurrentLatitude = vehicle.CurrentLatitude,
            CurrentLongitude = vehicle.CurrentLongitude,
            IsActive = vehicle.IsActive,
            CreatedAt = vehicle.CreatedAt
        };
    }

    public async Task<IEnumerable<VehicleDto>> GetAvailableVehiclesAsync()
    {
        return await _context.Vehicles
            .Where(v => v.IsActive && v.Status == VehicleStatus.Available)
            .Select(v => new VehicleDto
            {
                Id = v.Id,
                LicensePlate = v.LicensePlate,
                Brand = v.Brand,
                Model = v.Model,
                Year = v.Year,
                Capacity = v.Capacity,
                Status = v.Status,
                CurrentLatitude = v.CurrentLatitude,
                CurrentLongitude = v.CurrentLongitude,
                IsActive = v.IsActive,
                CreatedAt = v.CreatedAt
            })
            .OrderBy(v => v.LicensePlate)
            .ToListAsync();
    }
}