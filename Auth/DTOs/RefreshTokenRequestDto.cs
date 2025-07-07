using System.ComponentModel.DataAnnotations;

namespace FuelTrack.Api.Auth.DTOs;

public class RefreshTokenRequestDto
{
    [Required]
    public string RefreshToken { get; set; } = string.Empty;
}