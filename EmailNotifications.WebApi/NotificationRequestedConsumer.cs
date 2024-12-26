using Contracts;
using MassTransit;

namespace EmailNotifications.WebApi;

// ReSharper disable once UnusedType.Global
public class NotificationRequestedConsumer(
    ILogger<NotificationRequestedConsumer> logger) : IConsumer<NotificationRequested>
{
    public Task Consume(ConsumeContext<NotificationRequested> context)
    {
        var notificationRequested = context.Message;
        
        logger.LogInformation("{NotificationRequested} has been received", notificationRequested);

        return Task.CompletedTask;
    }
}