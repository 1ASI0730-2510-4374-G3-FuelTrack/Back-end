using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using FuelTrack.Api.Orders.DTOs;
using FuelTrack.Api.Orders.Services;

namespace FuelTrack.Api.Orders.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
[Tags("Orders")]
public class OrdersController : ControllerBase
{
    private readonly IOrderService _orderService;

    public OrdersController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    /// <summary>
    /// Obtener pedidos (Admin: todos, Cliente: propios, Proveedor: asignados)
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<OrderDto>>> GetOrders()
    {
        var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (!int.TryParse(userIdClaim, out int userId))
            return Unauthorized();

        IEnumerable<OrderDto> orders = userRole switch
        {
            "Admin" => await _orderService.GetOrdersAsync(),
            "Cliente" => await _orderService.GetOrdersAsync(userId),
            "Proveedor" => await _orderService.GetOrdersByProviderAsync(),
            _ => new List<OrderDto>()
        };

        return Ok(orders);
    }

    /// <summary>
    /// Obtener pedido por ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<OrderDto>> GetOrder(int id)
    {
        var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (!int.TryParse(userIdClaim, out int userId))
            return Unauthorized();

        var order = userRole switch
        {
            "Admin" => await _orderService.GetOrderByIdAsync(id),
            "Cliente" => await _orderService.GetOrderByIdAsync(id, userId),
            "Proveedor" => await _orderService.GetOrderByIdAsync(id),
            _ => null
        };

        if (order == null)
            return NotFound();

        return Ok(order);
    }

    /// <summary>
    /// Crear nuevo pedido (Solo Cliente)
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Cliente")]
    public async Task<ActionResult<OrderDto>> CreateOrder([FromBody] CreateOrderDto createDto)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (!int.TryParse(userIdClaim, out int userId))
            return Unauthorized();

        var order = await _orderService.CreateOrderAsync(createDto, userId);
        return CreatedAtAction(nameof(GetOrder), new { id = order.Id }, order);
    }

    /// <summary>
    /// Asignar vehículo y operador a pedido (Solo Proveedor)
    /// </summary>
    [HttpPost("{id}/assign")]
    [Authorize(Roles = "Proveedor")]
    public async Task<ActionResult<OrderDto>> AssignDelivery(int id, [FromBody] AssignDeliveryDto assignDto)
    {
        try
        {
            var order = await _orderService.AssignDeliveryAsync(id, assignDto);
            return Ok(order);
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
    /// Actualizar estado del pedido (Solo Proveedor)
    /// </summary>
    [HttpPatch("{id}/status")]
    [Authorize(Roles = "Proveedor")]
    public async Task<ActionResult<OrderDto>> UpdateOrderStatus(int id, [FromBody] int newStatus)
    {
        try
        {
            var order = await _orderService.UpdateOrderStatusAsync(id, newStatus);
            return Ok(order);
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
    /// Obtener pedidos asignados al proveedor
    /// </summary>
    [HttpGet("provider")]
    [Authorize(Roles = "Proveedor")]
    public async Task<ActionResult<IEnumerable<OrderDto>>> GetOrdersForProvider()
    {
        var orders = await _orderService.GetOrdersByProviderAsync();
        return Ok(orders);
    }

}