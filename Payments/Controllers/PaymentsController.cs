using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using FuelTrack.Api.Payments.DTOs;
using FuelTrack.Api.Payments.Services;

namespace FuelTrack.Api.Payments.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
[Tags("Payments")]
public class PaymentsController : ControllerBase
{
    private readonly IPaymentService _paymentService;

    public PaymentsController(IPaymentService paymentService)
    {
        _paymentService = paymentService;
    }

    /// <summary>
    /// Obtener métodos de pago del usuario actual
    /// </summary>
    [HttpGet("methods")]
    [Authorize(Roles = "Cliente")]
    public async Task<ActionResult<IEnumerable<PaymentMethodDto>>> GetPaymentMethods()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (!int.TryParse(userIdClaim, out int userId))
            return Unauthorized();

        var paymentMethods = await _paymentService.GetPaymentMethodsAsync(userId);
        return Ok(paymentMethods);
    }

    /// <summary>
    /// Registrar nuevo método de pago
    /// </summary>
    [HttpPost("methods")]
   [Authorize(Roles = "Cliente")]
   public async Task<ActionResult<PaymentMethodDto>> CreatePaymentMethod([FromBody] CreatePaymentMethodDto createDto)
{
    if (!ModelState.IsValid)
        return BadRequest(ModelState); // <----- VALIDA EL MODELO AQUÍ

    var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

    if (!int.TryParse(userIdClaim, out int userId))
        return Unauthorized();

    try
    {
        var paymentMethod = await _paymentService.CreatePaymentMethodAsync(createDto, userId);
        return CreatedAtAction(nameof(GetPaymentMethods), paymentMethod);
    }
    catch (InvalidOperationException ex)
    {
        return BadRequest(new { message = ex.Message });
    }
}


    /// <summary>
    /// Eliminar método de pago
    /// </summary>
    [HttpDelete("methods/{id}")]
    [Authorize(Roles = "Cliente")]
    public async Task<IActionResult> DeletePaymentMethod(int id)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (!int.TryParse(userIdClaim, out int userId))
            return Unauthorized();

        var result = await _paymentService.DeletePaymentMethodAsync(id, userId);
        
        if (!result)
            return NotFound();

        return Ok(new { message = "Método de pago eliminado" });
    }

    /// <summary>
    /// Procesar pago de pedido
    /// </summary>
    [HttpPost("process")]
    [Authorize(Roles = "Cliente")]
    public async Task<ActionResult<PaymentDto>> ProcessPayment([FromBody] ProcessPaymentDto processDto)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (!int.TryParse(userIdClaim, out int userId))
            return Unauthorized();

        try
        {
            var payment = await _paymentService.ProcessPaymentAsync(processDto, userId);
            return Ok(payment);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Obtener historial de pagos
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<PaymentDto>>> GetPayments()
    {
        var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (!int.TryParse(userIdClaim, out int userId))
            return Unauthorized();

        IEnumerable<PaymentDto> payments = userRole switch
        {
            "Admin" => await _paymentService.GetPaymentsAsync(),
            "Cliente" => await _paymentService.GetPaymentsAsync(userId),
            _ => new List<PaymentDto>()
        };

        return Ok(payments);
    }

    /// <summary>
    /// Obtener pago por ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<PaymentDto>> GetPayment(int id)
    {
        var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (!int.TryParse(userIdClaim, out int userId))
            return Unauthorized();

        var payment = userRole switch
        {
            "Admin" => await _paymentService.GetPaymentByIdAsync(id),
            "Cliente" => await _paymentService.GetPaymentByIdAsync(id, userId),
            _ => null
        };

        if (payment == null)
            return NotFound();

        return Ok(payment);
    }
}
