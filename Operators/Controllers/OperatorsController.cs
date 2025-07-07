using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FuelTrack.Api.Operators.DTOs;
using FuelTrack.Api.Operators.Services;

namespace FuelTrack.Api.Operators.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
[Tags("Operators")]
public class OperatorsController : ControllerBase
{
    private readonly IOperatorService _operatorService;

    public OperatorsController(IOperatorService operatorService)
    {
        _operatorService = operatorService;
    }

    /// <summary>
    /// Obtener todos los operadores
    /// </summary>
    [HttpGet]
    [Authorize(Roles = "Admin,Proveedor")]
    public async Task<ActionResult<IEnumerable<OperatorDto>>> GetOperators()
    {
        var operators = await _operatorService.GetOperatorsAsync();
        return Ok(operators);
    }

    /// <summary>
    /// Obtener operadores disponibles
    /// </summary>
    [HttpGet("available")]
    [Authorize(Roles = "Proveedor")]
    public async Task<ActionResult<IEnumerable<OperatorDto>>> GetAvailableOperators()
    {
        var operators = await _operatorService.GetAvailableOperatorsAsync();
        return Ok(operators);
    }

    /// <summary>
    /// Obtener operador por ID
    /// </summary>
    [HttpGet("{id}")]
    [Authorize(Roles = "Admin,Proveedor")]
    public async Task<ActionResult<OperatorDto>> GetOperator(int id)
    {
        var operatorDto = await _operatorService.GetOperatorByIdAsync(id);
        
        if (operatorDto == null)
            return NotFound();
            
        return Ok(operatorDto);
    }

    /// <summary>
    /// Crear nuevo operador (Solo Admin)
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<OperatorDto>> CreateOperator([FromBody] CreateOperatorDto createDto)
    {
        try
        {
            var operatorDto = await _operatorService.CreateOperatorAsync(createDto);
            return CreatedAtAction(nameof(GetOperator), new { id = operatorDto.Id }, operatorDto);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Actualizar operador (Solo Admin)
    /// </summary>
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<OperatorDto>> UpdateOperator(int id, [FromBody] CreateOperatorDto updateDto)
    {
        try
        {
            var operatorDto = await _operatorService.UpdateOperatorAsync(id, updateDto);
            return Ok(operatorDto);
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
    /// Eliminar operador (Solo Admin)
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteOperator(int id)
    {
        try
        {
            var result = await _operatorService.DeleteOperatorAsync(id);
            
            if (!result)
                return NotFound();
                
            return Ok(new { message = "Operador eliminado" });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}