using Microsoft.EntityFrameworkCore;
using FuelTrack.Api.Shared.Data;
using FuelTrack.Api.Shared.Models;
using FuelTrack.Api.Operators.DTOs;

namespace FuelTrack.Api.Operators.Services;

public class OperatorService : IOperatorService
{
    private readonly FuelTrackDbContext _context;

    public OperatorService(FuelTrackDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<OperatorDto>> GetOperatorsAsync()
    {
        return await _context.Operators
            .Where(o => o.IsActive)
            .Select(o => new OperatorDto
            {
                Id = o.Id,
                FirstName = o.FirstName,
                LastName = o.LastName,
                LicenseNumber = o.LicenseNumber,
                LicenseExpiryDate = o.LicenseExpiryDate,
                Phone = o.Phone,
                Status = o.Status,
                IsActive = o.IsActive,
                CreatedAt = o.CreatedAt
            })
            .OrderBy(o => o.FirstName)
            .ThenBy(o => o.LastName)
            .ToListAsync();
    }

    public async Task<OperatorDto?> GetOperatorByIdAsync(int id)
    {
        var operatorEntity = await _context.Operators
            .Where(o => o.Id == id && o.IsActive)
            .FirstOrDefaultAsync();

        if (operatorEntity == null)
            return null;

        return new OperatorDto
        {
            Id = operatorEntity.Id,
            FirstName = operatorEntity.FirstName,
            LastName = operatorEntity.LastName,
            LicenseNumber = operatorEntity.LicenseNumber,
            LicenseExpiryDate = operatorEntity.LicenseExpiryDate,
            Phone = operatorEntity.Phone,
            Status = operatorEntity.Status,
            IsActive = operatorEntity.IsActive,
            CreatedAt = operatorEntity.CreatedAt
        };
    }

    public async Task<OperatorDto> CreateOperatorAsync(CreateOperatorDto createDto)
    {
        if (await _context.Operators.AnyAsync(o => o.LicenseNumber == createDto.LicenseNumber && o.IsActive))
        {
            throw new InvalidOperationException("Ya existe un operador con este número de licencia");
        }

        if (createDto.LicenseExpiryDate <= DateTime.UtcNow)
        {
            throw new InvalidOperationException("La fecha de vencimiento de la licencia debe ser futura");
        }

        var operatorEntity = new Operator
        {
            FirstName = createDto.FirstName,
            LastName = createDto.LastName,
            LicenseNumber = createDto.LicenseNumber,
            LicenseExpiryDate = createDto.LicenseExpiryDate,
            Phone = createDto.Phone,
            Status = OperatorStatus.Available
        };

        _context.Operators.Add(operatorEntity);
        await _context.SaveChangesAsync();

        return new OperatorDto
        {
            Id = operatorEntity.Id,
            FirstName = operatorEntity.FirstName,
            LastName = operatorEntity.LastName,
            LicenseNumber = operatorEntity.LicenseNumber,
            LicenseExpiryDate = operatorEntity.LicenseExpiryDate,
            Phone = operatorEntity.Phone,
            Status = operatorEntity.Status,
            IsActive = operatorEntity.IsActive,
            CreatedAt = operatorEntity.CreatedAt
        };
    }

    public async Task<OperatorDto> UpdateOperatorAsync(int id, CreateOperatorDto updateDto)
    {
        var operatorEntity = await _context.Operators.FindAsync(id);

        if (operatorEntity == null || !operatorEntity.IsActive)
            throw new KeyNotFoundException("Operador no encontrado");

        if (await _context.Operators.AnyAsync(o => o.LicenseNumber == updateDto.LicenseNumber && o.Id != id && o.IsActive))
        {
            throw new InvalidOperationException("Ya existe un operador con este número de licencia");
        }

        if (updateDto.LicenseExpiryDate <= DateTime.UtcNow)
        {
            throw new InvalidOperationException("La fecha de vencimiento de la licencia debe ser futura");
        }

        operatorEntity.FirstName = updateDto.FirstName;
        operatorEntity.LastName = updateDto.LastName;
        operatorEntity.LicenseNumber = updateDto.LicenseNumber;
        operatorEntity.LicenseExpiryDate = updateDto.LicenseExpiryDate;
        operatorEntity.Phone = updateDto.Phone;
        operatorEntity.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return new OperatorDto
        {
            Id = operatorEntity.Id,
            FirstName = operatorEntity.FirstName,
            LastName = operatorEntity.LastName,
            LicenseNumber = operatorEntity.LicenseNumber,
            LicenseExpiryDate = operatorEntity.LicenseExpiryDate,
            Phone = operatorEntity.Phone,
            Status = operatorEntity.Status,
            IsActive = operatorEntity.IsActive,
            CreatedAt = operatorEntity.CreatedAt
        };
    }

    public async Task<bool> DeleteOperatorAsync(int id)
    {
        var operatorEntity = await _context.Operators.FindAsync(id);

        if (operatorEntity == null || !operatorEntity.IsActive)
            return false;

        // Check if operator is currently assigned to any active order
        var hasActiveOrders = await _context.Orders
            .AnyAsync(o => o.AssignedOperatorId == id && 
                          (o.Status == OrderStatus.Confirmed || o.Status == OrderStatus.InTransit));

        if (hasActiveOrders)
            throw new InvalidOperationException("No se puede eliminar un operador con pedidos activos");

        operatorEntity.IsActive = false;
        operatorEntity.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<IEnumerable<OperatorDto>> GetAvailableOperatorsAsync()
    {
        return await _context.Operators
            .Where(o => o.IsActive && o.Status == OperatorStatus.Available && o.LicenseExpiryDate > DateTime.UtcNow)
            .Select(o => new OperatorDto
            {
                Id = o.Id,
                FirstName = o.FirstName,
                LastName = o.LastName,
                LicenseNumber = o.LicenseNumber,
                LicenseExpiryDate = o.LicenseExpiryDate,
                Phone = o.Phone,
                Status = o.Status,
                IsActive = o.IsActive,
                CreatedAt = o.CreatedAt
            })
            .OrderBy(o => o.FirstName)
            .ThenBy(o => o.LastName)
            .ToListAsync();
    }
}