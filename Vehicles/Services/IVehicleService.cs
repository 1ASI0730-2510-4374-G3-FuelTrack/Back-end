using FuelTrack.Api.Vehicles.DTOs;

namespace FuelTrack.Api.Vehicles.Services;

public interface IVehicleService
{
    Task<IEnumerable<VehicleDto>> GetVehiclesAsync();
    Task<VehicleDto?> GetVehicleByIdAsync(int id);
    Task<VehicleDto> CreateVehicleAsync(CreateVehicleDto createDto);
    Task<VehicleDto> UpdateVehicleAsync(int id, CreateVehicleDto updateDto);
    Task<bool> DeleteVehicleAsync(int id);
    Task<VehicleDto> UpdateVehicleLocationAsync(int id, UpdateVehicleLocationDto locationDto);
    Task<IEnumerable<VehicleDto>> GetAvailableVehiclesAsync();
}