using FuelTrack.Api.Orders.DTOs;

namespace FuelTrack.Api.Orders.Services;

public interface IOrderService
{
    Task<IEnumerable<OrderDto>> GetOrdersAsync(int? userId = null);
    Task<OrderDto?> GetOrderByIdAsync(int id, int? userId = null);
    Task<OrderDto> CreateOrderAsync(CreateOrderDto createDto, int userId);
    Task<OrderDto> AssignDeliveryAsync(int orderId, AssignDeliveryDto assignDto);
    Task<OrderDto> UpdateOrderStatusAsync(int orderId, int newStatus);
    Task<IEnumerable<OrderDto>> GetOrdersByProviderAsync();
}