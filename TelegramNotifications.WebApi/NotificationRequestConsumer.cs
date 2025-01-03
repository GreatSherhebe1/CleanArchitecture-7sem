using Contracts;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace TelegramNotifications.WebApi
{
    public class NotificationRequestedConsumer(
    ILogger<NotificationRequestedConsumer> logger) : IConsumer<NotificationRequested>
    {
        public Task Consume(ConsumeContext<NotificationRequested> context)
        {
            var msg = context.Message;

            logger.LogInformation("{NotificationRequested} has been received", msg);
            return Task.CompletedTask;
        }
    }
}
