using FluentValidation;
using FuelTrack.Api.Auth.DTOs;

namespace FuelTrack.Api.Auth.Validators;

public class LoginRequestValidator : AbstractValidator<LoginRequestDto>
{
    public LoginRequestValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email es requerido")
            .EmailAddress().WithMessage("Email debe tener un formato válido");
            
        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password es requerido")
            .MinimumLength(6).WithMessage("Password debe tener al menos 6 caracteres");
    }
}