using Microsoft.AspNetCore.Mvc;
using NotificationStatusTracker.DAL.Models;
using NotificationStatusTracker.DAL.Repositories;

namespace NotificationStatusTracker.WebApi.Controllers;

[ApiController]
[Route("[controller]")]
public class NotificationsController(INotificationRepository repository) : ControllerBase
{
    [HttpGet]
    public IAsyncEnumerable<Notification> GetNotifications() => 
        repository.GetNotificationsAsync();
}