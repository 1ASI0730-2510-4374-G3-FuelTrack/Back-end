using Microsoft.EntityFrameworkCore;
using FuelTrack.Api.Shared.Data;
using FuelTrack.Api.Shared.Models;
using FuelTrack.Api.Orders.DTOs;

namespace FuelTrack.Api.Orders.Services;

public class OrderService : IOrderService
{
    private readonly FuelTrackDbContext _context;

    public OrderService(FuelTrackDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<OrderDto>> GetOrdersAsync(int? userId = null)
    {
        var query = _context.Orders
            .Include(o => o.User)
            .Include(o => o.AssignedVehicle)
            .Include(o => o.AssignedOperator)
            .AsQueryable();

        if (userId.HasValue)
        {
            query = query.Where(o => o.UserId == userId.Value);
        }

        return await query
            .Select(o => new OrderDto
            {
                Id = o.Id,
                OrderNumber = o.OrderNumber,
                FuelType = o.FuelType,
                Quantity = o.Quantity,
                PricePerLiter = o.PricePerLiter,
                TotalAmount = o.TotalAmount,
                Status = o.Status,
                DeliveryAddress = o.DeliveryAddress,
                DeliveryLatitude = o.DeliveryLatitude,
                DeliveryLongitude = o.DeliveryLongitude,
                EstimatedDeliveryTime = o.EstimatedDeliveryTime,
                ActualDeliveryTime = o.ActualDeliveryTime,
                CreatedAt = o.CreatedAt,
                CustomerName = $"{o.User.FirstName} {o.User.LastName}",
                AssignedVehiclePlate = o.AssignedVehicle != null ? o.AssignedVehicle.LicensePlate : null,
                AssignedOperatorName = o.AssignedOperator != null ? $"{o.AssignedOperator.FirstName} {o.AssignedOperator.LastName}" : null
            })
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync();
    }

    public async Task<OrderDto?> GetOrderByIdAsync(int id, int? userId = null)
    {
        var query = _context.Orders
            .Include(o => o.User)
            .Include(o => o.AssignedVehicle)
            .Include(o => o.AssignedOperator)
            .Where(o => o.Id == id);

        if (userId.HasValue)
        {
            query = query.Where(o => o.UserId == userId.Value);
        }

        var order = await query.FirstOrDefaultAsync();

        if (order == null)
            return null;

        return new OrderDto
        {
            Id = order.Id,
            OrderNumber = order.OrderNumber,
            FuelType = order.FuelType,
            Quantity = order.Quantity,
            PricePerLiter = order.PricePerLiter,
            TotalAmount = order.TotalAmount,
            Status = order.Status,
            DeliveryAddress = order.DeliveryAddress,
            DeliveryLatitude = order.DeliveryLatitude,
            DeliveryLongitude = order.DeliveryLongitude,
            EstimatedDeliveryTime = order.EstimatedDeliveryTime,
            ActualDeliveryTime = order.ActualDeliveryTime,
            CreatedAt = order.CreatedAt,
            CustomerName = $"{order.User.FirstName} {order.User.LastName}",
            AssignedVehiclePlate = order.AssignedVehicle?.LicensePlate,
            AssignedOperatorName = order.AssignedOperator != null ? $"{order.AssignedOperator.FirstName} {order.AssignedOperator.LastName}" : null
        };
    }

    public async Task<OrderDto> CreateOrderAsync(CreateOrderDto createDto, int userId)
    {
        // Get fuel price (this would typically come from a pricing service)
        var pricePerLiter = GetFuelPrice(createDto.FuelType);
        var totalAmount = createDto.Quantity * pricePerLiter;

        var order = new Order
        {
            UserId = userId,
            OrderNumber = GenerateOrderNumber(),
            FuelType = createDto.FuelType,
            Quantity = createDto.Quantity,
            PricePerLiter = pricePerLiter,
            TotalAmount = totalAmount,
            DeliveryAddress = createDto.DeliveryAddress,
            DeliveryLatitude = createDto.DeliveryLatitude,
            DeliveryLongitude = createDto.DeliveryLongitude,
            Status = OrderStatus.Pending
        };

        _context.Orders.Add(order);
        await _context.SaveChangesAsync();

        return await GetOrderByIdAsync(order.Id) ?? throw new InvalidOperationException("Error creating order");
    }

    public async Task<OrderDto> AssignDeliveryAsync(int orderId, AssignDeliveryDto assignDto)
    {
        var order = await _context.Orders.FindAsync(orderId);
        
        if (order == null)
            throw new KeyNotFoundException("Pedido no encontrado");

        // Verify vehicle and operator exist and are available
        var vehicle = await _context.Vehicles.FindAsync(assignDto.VehicleId);
        var operatorEntity = await _context.Operators.FindAsync(assignDto.OperatorId);

        if (vehicle == null || operatorEntity == null)
            throw new InvalidOperationException("Vehículo u operador no encontrado");

        if (vehicle.Status != VehicleStatus.Available || operatorEntity.Status != OperatorStatus.Available)
            throw new InvalidOperationException("Vehículo u operador no disponible");

        order.AssignedVehicleId = assignDto.VehicleId;
        order.AssignedOperatorId = assignDto.OperatorId;
        order.EstimatedDeliveryTime = assignDto.EstimatedDeliveryTime;
        order.Status = OrderStatus.Confirmed;
        order.UpdatedAt = DateTime.UtcNow;

        // Update vehicle and operator status
        vehicle.Status = VehicleStatus.InUse;
        operatorEntity.Status = OperatorStatus.OnDelivery;

        await _context.SaveChangesAsync();

        return await GetOrderByIdAsync(orderId) ?? throw new InvalidOperationException("Error updating order");
    }

    public async Task<OrderDto> UpdateOrderStatusAsync(int orderId, int newStatus)
    {
        var order = await _context.Orders
            .Include(o => o.AssignedVehicle)
            .Include(o => o.AssignedOperator)
            .FirstOrDefaultAsync(o => o.Id == orderId);
        
        if (order == null)
            throw new KeyNotFoundException("Pedido no encontrado");

        var orderStatus = (OrderStatus)newStatus;
        order.Status = orderStatus;
        order.UpdatedAt = DateTime.UtcNow;

        // If order is completed, update delivery time and free resources
        if (orderStatus == OrderStatus.Delivered)
        {
            order.ActualDeliveryTime = DateTime.UtcNow;
            
            if (order.AssignedVehicle != null)
                order.AssignedVehicle.Status = VehicleStatus.Available;
                
            if (order.AssignedOperator != null)
                order.AssignedOperator.Status = OperatorStatus.Available;
        }

        await _context.SaveChangesAsync();

        return await GetOrderByIdAsync(orderId) ?? throw new InvalidOperationException("Error updating order");
    }

    public async Task<IEnumerable<OrderDto>> GetOrdersByProviderAsync()
    {
        return await _context.Orders
            .Include(o => o.User)
            .Include(o => o.AssignedVehicle)
            .Include(o => o.AssignedOperator)
            .Where(o => o.Status == OrderStatus.Pending || o.Status == OrderStatus.Confirmed || o.Status == OrderStatus.InTransit)
            .Select(o => new OrderDto
            {
                Id = o.Id,
                OrderNumber = o.OrderNumber,
                FuelType = o.FuelType,
                Quantity = o.Quantity,
                PricePerLiter = o.PricePerLiter,
                TotalAmount = o.TotalAmount,
                Status = o.Status,
                DeliveryAddress = o.DeliveryAddress,
                DeliveryLatitude = o.DeliveryLatitude,
                DeliveryLongitude = o.DeliveryLongitude,
                EstimatedDeliveryTime = o.EstimatedDeliveryTime,
                ActualDeliveryTime = o.ActualDeliveryTime,
                CreatedAt = o.CreatedAt,
                CustomerName = $"{o.User.FirstName} {o.User.LastName}",
                AssignedVehiclePlate = o.AssignedVehicle != null ? o.AssignedVehicle.LicensePlate : null,
                AssignedOperatorName = o.AssignedOperator != null ? $"{o.AssignedOperator.FirstName} {o.AssignedOperator.LastName}" : null
            })
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync();
    }

    private static decimal GetFuelPrice(FuelType fuelType)
    {
        return fuelType switch
        {
            FuelType.Gasoline => 1.25m,
            FuelType.Diesel => 1.15m,
            FuelType.Premium => 1.45m,
            _ => 1.25m
        };
    }

    private static string GenerateOrderNumber()
    {
        return $"ORD-{DateTime.UtcNow:yyyyMMdd}-{Random.Shared.Next(1000, 9999)}";
    }
}