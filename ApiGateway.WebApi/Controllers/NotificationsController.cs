using ApiGateway.WebApi.DTOs;
using MassTransit;
using Microsoft.AspNetCore.Mvc;

namespace ApiGateway.WebApi.Controllers;

[ApiController]
[Route("[controller]")]
public class NotificationsController(
    ILogger<NotificationsController> logger,
    IPublishEndpoint publishEndpoint) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> CreateNotification(
        [FromBody] CreateNotificationRequest notificationRequest)
    {
        var contracts = notificationRequest.MapToContracts();

        logger.LogInformation("Creating notification {CreateNotificationRequest}", notificationRequest);

        foreach (var contract in contracts)
            await publishEndpoint.Publish(contract);

        return Ok();
    }
}