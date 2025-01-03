using Contracts;
using Microsoft.EntityFrameworkCore;
using NotificationStatusTracker.DAL.Models;

namespace NotificationStatusTracker.DAL.Repositories;

public class DbNotificationRepository(NotificationContext context) : INotificationRepository
{
    public IAsyncEnumerable<Notification> GetNotificationsAsync() => 
        context.Notifications.AsAsyncEnumerable();

    public Task<Notification?> FindNotificationAsync(Guid guid) => 
        context.Notifications.FirstOrDefaultAsync(n => n.Guid == guid);

    public async Task<Notification> AddNotificationAsync(Notification notification)
    {
        var entityEntry = await context.Notifications.AddAsync(notification);

        await context.SaveChangesAsync();
        return entityEntry.Entity;
    }

    public async Task<Notification> UpdateNotificationStatusAsync(Notification notification, NotificationStatus notificationStatus)
    {
        notification = await context.Notifications.FindAsync(notification.Id) ??
                       throw new InvalidOperationException($"Notification with id {notification.Id} not found");
        
        
        
        throw new NotImplementedException();
    }
}