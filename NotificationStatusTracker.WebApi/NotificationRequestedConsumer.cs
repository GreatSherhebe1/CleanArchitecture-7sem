using Contracts;
using MassTransit;
using NotificationStatusTracker.DAL.Repositories;

namespace NotificationStatusTracker.WebApi;

// ReSharper disable once UnusedType.Global
public class NotificationRequestedConsumer(
    ILogger<NotificationRequestedConsumer> logger,
    INotificationRepository notificationRepository) : IConsumer<NotificationRequested>
{
    public async Task Consume(ConsumeContext<NotificationRequested> context)
    {
        var notificationRequested = context.Message;

        logger.LogInformation("{NotificationRequested} has been received", notificationRequested);

        _ = await notificationRepository.FindNotificationAsync(notificationRequested.Id) ??
            await notificationRepository.AddNotificationAsync(notificationRequested.ToNotification());
    }
}