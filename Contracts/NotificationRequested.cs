namespace Contracts;

public record NotificationRequested
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public required string Channel { get; init; }
    public required string Text { get; init; }
    public required NotificationStatus Status { get; init; }
    public required IReadOnlyCollection<string> Recipients { get; init; }
}