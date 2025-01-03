using Contracts;
using NotificationStatusTracker.DAL.Models;

namespace NotificationStatusTracker.WebApi;

public static class Extensions
{
    public static Notification ToNotification(this NotificationRequested notificationRequested) => new()
    {
        Guid = notificationRequested.Id,
        Channel = notificationRequested.Channel,
        Text = notificationRequested.Text,
        Recipients = notificationRequested.Recipients.ToArray()
    };
}