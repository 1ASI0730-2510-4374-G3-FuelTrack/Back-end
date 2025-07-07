using Microsoft.EntityFrameworkCore;
using FuelTrack.Api.Shared.Data;
using FuelTrack.Api.Shared.Models;
using FuelTrack.Api.Notifications.DTOs;

namespace FuelTrack.Api.Notifications.Services;

public class NotificationService : INotificationService
{
    private readonly FuelTrackDbContext _context;

    public NotificationService(FuelTrackDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<NotificationDto>> GetNotificationsAsync(int userId)
    {
        return await _context.Notifications
            .Include(n => n.RelatedOrder)
            .Where(n => n.UserId == userId && n.IsActive)
            .Select(n => new NotificationDto
            {
                Id = n.Id,
                Title = n.Title,
                Message = n.Message,
                Type = n.Type,
                IsRead = n.IsRead,
                RelatedOrderId = n.RelatedOrderId,
                RelatedOrderNumber = n.RelatedOrder != null ? n.RelatedOrder.OrderNumber : null,
                CreatedAt = n.CreatedAt
            })
            .OrderByDescending(n => n.CreatedAt)
            .ToListAsync();
    }

    public async Task<NotificationDto?> GetNotificationByIdAsync(int id, int userId)
    {
        var notification = await _context.Notifications
            .Include(n => n.RelatedOrder)
            .Where(n => n.Id == id && n.UserId == userId && n.IsActive)
            .FirstOrDefaultAsync();

        if (notification == null)
            return null;

        return new NotificationDto
        {
            Id = notification.Id,
            Title = notification.Title,
            Message = notification.Message,
            Type = notification.Type,
            IsRead = notification.IsRead,
            RelatedOrderId = notification.RelatedOrderId,
            RelatedOrderNumber = notification.RelatedOrder?.OrderNumber,
            CreatedAt = notification.CreatedAt
        };
    }

    public async Task<NotificationDto> CreateNotificationAsync(CreateNotificationDto createDto)
    {
        var notification = new Notification
        {
            UserId = createDto.UserId,
            Title = createDto.Title,
            Message = createDto.Message,
            Type = createDto.Type,
            RelatedOrderId = createDto.RelatedOrderId
        };

        _context.Notifications.Add(notification);
        await _context.SaveChangesAsync();

        // Load related order if exists
        if (notification.RelatedOrderId.HasValue)
        {
            await _context.Entry(notification)
                .Reference(n => n.RelatedOrder)
                .LoadAsync();
        }

        return new NotificationDto
        {
            Id = notification.Id,
            Title = notification.Title,
            Message = notification.Message,
            Type = notification.Type,
            IsRead = notification.IsRead,
            RelatedOrderId = notification.RelatedOrderId,
            RelatedOrderNumber = notification.RelatedOrder?.OrderNumber,
            CreatedAt = notification.CreatedAt
        };
    }

    public async Task<bool> MarkAsReadAsync(int id, int userId)
    {
        var notification = await _context.Notifications
            .FirstOrDefaultAsync(n => n.Id == id && n.UserId == userId && n.IsActive);

        if (notification == null)
            return false;

        notification.IsRead = true;
        notification.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> MarkAllAsReadAsync(int userId)
    {
        var notifications = await _context.Notifications
            .Where(n => n.UserId == userId && !n.IsRead && n.IsActive)
            .ToListAsync();

        if (!notifications.Any())
            return false;

        foreach (var notification in notifications)
        {
            notification.IsRead = true;
            notification.UpdatedAt = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<int> GetUnreadCountAsync(int userId)
    {
        return await _context.Notifications
            .CountAsync(n => n.UserId == userId && !n.IsRead && n.IsActive);
    }
}