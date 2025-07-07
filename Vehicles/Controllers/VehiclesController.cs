using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FuelTrack.Api.Vehicles.DTOs;
using FuelTrack.Api.Vehicles.Services;

namespace FuelTrack.Api.Vehicles.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
[Tags("Vehicles")]
public class VehiclesController : ControllerBase
{
    private readonly IVehicleService _vehicleService;

    public VehiclesController(IVehicleService vehicleService)
    {
        _vehicleService = vehicleService;
    }

    /// <summary>
    /// Obtener todos los vehículos
    /// </summary>
    [HttpGet]
    [Authorize(Roles = "Admin,Proveedor")]
    public async Task<ActionResult<IEnumerable<VehicleDto>>> GetVehicles()
    {
        var vehicles = await _vehicleService.GetVehiclesAsync();
        return Ok(vehicles);
    }

    /// <summary>
    /// Obtener vehículos disponibles
    /// </summary>
    [HttpGet("available")]
    [Authorize(Roles = "Proveedor")]
    public async Task<ActionResult<IEnumerable<VehicleDto>>> GetAvailableVehicles()
    {
        var vehicles = await _vehicleService.GetAvailableVehiclesAsync();
        return Ok(vehicles);
    }

    /// <summary>
    /// Obtener vehículo por ID
    /// </summary>
    [HttpGet("{id}")]
    [Authorize(Roles = "Admin,Proveedor")]
    public async Task<ActionResult<VehicleDto>> GetVehicle(int id)
    {
        var vehicle = await _vehicleService.GetVehicleByIdAsync(id);
        
        if (vehicle == null)
            return NotFound();
            
        return Ok(vehicle);
    }

    /// <summary>
    /// Crear nuevo vehículo (Solo Admin)
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<VehicleDto>> CreateVehicle([FromBody] CreateVehicleDto createDto)
    {
        try
        {
            var vehicle = await _vehicleService.CreateVehicleAsync(createDto);
            return CreatedAtAction(nameof(GetVehicle), new { id = vehicle.Id }, vehicle);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Actualizar vehículo (Solo Admin)
    /// </summary>
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<VehicleDto>> UpdateVehicle(int id, [FromBody] CreateVehicleDto updateDto)
    {
        try
        {
            var vehicle = await _vehicleService.UpdateVehicleAsync(id, updateDto);
            return Ok(vehicle);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Eliminar vehículo (Solo Admin)
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteVehicle(int id)
    {
        try
        {
            var result = await _vehicleService.DeleteVehicleAsync(id);
            
            if (!result)
                return NotFound();
                
            return Ok(new { message = "Vehículo eliminado" });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Actualizar ubicación del vehículo (Solo Proveedor)
    /// </summary>
    [HttpPatch("{id}/location")]
    [Authorize(Roles = "Proveedor")]
    public async Task<ActionResult<VehicleDto>> UpdateVehicleLocation(int id, [FromBody] UpdateVehicleLocationDto locationDto)
    {
        try
        {
            var vehicle = await _vehicleService.UpdateVehicleLocationAsync(id, locationDto);
            return Ok(vehicle);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }
}