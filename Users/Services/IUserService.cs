using FuelTrack.Api.Users.DTOs;

namespace FuelTrack.Api.Users.Services;

public interface IUserService
{
    Task<IEnumerable<UserDto>> GetAllUsersAsync();
    Task<UserDto?> GetUserByIdAsync(int id);
    Task<UserDto> UpdateUserAsync(int id, UpdateUserDto updateDto);
    Task<bool> ToggleUserStatusAsync(int id);
    Task<UserDto?> GetCurrentUserAsync(int userId);
    
}