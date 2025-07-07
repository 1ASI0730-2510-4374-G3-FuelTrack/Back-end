using FluentValidation;
using FuelTrack.Api.Auth.DTOs;
using FuelTrack.Api.Shared.Models;

namespace FuelTrack.Api.Auth.Validators;

public class RegisterRequestValidator : AbstractValidator<RegisterRequestDto>
{
    public RegisterRequestValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("Nombre es requerido")
            .MaximumLength(100).WithMessage("Nombre no puede exceder 100 caracteres");
            
        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Apellido es requerido")
            .MaximumLength(100).WithMessage("Apellido no puede exceder 100 caracteres");
            
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email es requerido")
            .EmailAddress().WithMessage("Email debe tener un formato válido");
            
        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password es requerido")
            .MinimumLength(6).WithMessage("Password debe tener al menos 6 caracteres")
            .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]")
            .WithMessage("Password debe contener al menos una mayúscula, una minúscula, un número y un carácter especial");
            
        RuleFor(x => x.Role)
            .IsInEnum().WithMessage("Rol debe ser válido (Admin, Cliente, Proveedor)");
            
        RuleFor(x => x.Phone)
            .MaximumLength(20).WithMessage("Teléfono no puede exceder 20 caracteres")
            .When(x => !string.IsNullOrEmpty(x.Phone));
    }
}