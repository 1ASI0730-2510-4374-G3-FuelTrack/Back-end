using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using FuelTrack.Api.Users.DTOs;
using FuelTrack.Api.Users.Services;

namespace FuelTrack.Api.Users.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
[Tags("Users")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    /// <summary>
    /// Obtener todos los usuarios (Solo Admin)
    /// </summary>
    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<IEnumerable<UserDto>>> GetAllUsers()
    {
        var users = await _userService.GetAllUsersAsync();
        return Ok(users);
    }

    /// <summary>
    /// Obtener usuario por ID
    /// </summary>
    [HttpGet("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<UserDto>> GetUser(int id)
    {
        var user = await _userService.GetUserByIdAsync(id);
        
        if (user == null)
            return NotFound();
            
        return Ok(user);
    }

    /// <summary>
    /// Obtener perfil del usuario actual
    /// </summary>
    [HttpGet("profile")]
    public async Task<ActionResult<UserDto>> GetCurrentUserProfile()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        
        if (!int.TryParse(userIdClaim, out int userId))
            return Unauthorized();

        var user = await _userService.GetCurrentUserAsync(userId);
        
        if (user == null)
            return NotFound();
            
        return Ok(user);
    }

    /// <summary>
    /// Actualizar perfil del usuario actual
    /// </summary>
    [HttpPut("profile")]
    public async Task<ActionResult<UserDto>> UpdateCurrentUserProfile([FromBody] UpdateUserDto updateDto)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        
        if (!int.TryParse(userIdClaim, out int userId))
            return Unauthorized();

        try
        {
            var updatedUser = await _userService.UpdateUserAsync(userId, updateDto);
            return Ok(updatedUser);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    /// <summary>
    /// Activar/Desactivar usuario (Solo Admin)
    /// </summary>
    [HttpPatch("{id}/toggle-status")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> ToggleUserStatus(int id)
    {
        var result = await _userService.ToggleUserStatusAsync(id);
        
        if (!result)
            return NotFound();
            
        return Ok(new { message = "Estado del usuario actualizado" });
    }
}