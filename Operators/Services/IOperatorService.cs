using FuelTrack.Api.Operators.DTOs;

namespace FuelTrack.Api.Operators.Services;

public interface IOperatorService
{
    Task<IEnumerable<OperatorDto>> GetOperatorsAsync();
    Task<OperatorDto?> GetOperatorByIdAsync(int id);
    Task<OperatorDto> CreateOperatorAsync(CreateOperatorDto createDto);
    Task<OperatorDto> UpdateOperatorAsync(int id, CreateOperatorDto updateDto);
    Task<bool> DeleteOperatorAsync(int id);
    Task<IEnumerable<OperatorDto>> GetAvailableOperatorsAsync();
}