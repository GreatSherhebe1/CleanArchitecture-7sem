using Contracts;
using NotificationStatusTracker.DAL.Models;

namespace NotificationStatusTracker.DAL.Repositories;

public interface INotificationRepository
{
    public IAsyncEnumerable<Notification> GetNotificationsAsync();
    public Task<Notification?> FindNotificationAsync(Guid guid);
    public Task<Notification> AddNotificationAsync(Notification notification);

    public Task<Notification> UpdateNotificationStatusAsync(
        Notification notification,
        NotificationStatus notificationStatus);
}