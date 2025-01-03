using System.Collections.Immutable;
using Contracts;

namespace ApiGateway.WebApi.DTOs;

public record CreateNotificationRequest
{
    public required ImmutableHashSet<string> Channels { get; init; } = [];
    
    public required string Text { get; init; } = "";
    public required ImmutableHashSet<string> Recipients { get; init; } = [];

    public IEnumerable<NotificationRequested> MapToContracts()
    {
        return Channels.Select(channel => new NotificationRequested
        {
            Channel = channel,
            Text = Text,
            Recipients = Recipients,
            Status = NotificationStatus.Created
        });
    }
}