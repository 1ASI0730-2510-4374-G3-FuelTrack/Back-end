using FuelTrack.Api.Notifications.DTOs;

namespace FuelTrack.Api.Notifications.Services;

public interface INotificationService
{
    Task<IEnumerable<NotificationDto>> GetNotificationsAsync(int userId);
    Task<NotificationDto?> GetNotificationByIdAsync(int id, int userId);
    Task<NotificationDto> CreateNotificationAsync(CreateNotificationDto createDto);
    Task<bool> MarkAsReadAsync(int id, int userId);
    Task<bool> MarkAllAsReadAsync(int userId);
    Task<int> GetUnreadCountAsync(int userId);
}