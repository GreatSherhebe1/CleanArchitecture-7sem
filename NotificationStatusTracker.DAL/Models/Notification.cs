using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations;
using Contracts;

namespace NotificationStatusTracker.DAL.Models;

public class Notification
{
    [Key]
    public int Id { get; init; }
    public Guid Guid { get; init; } = Guid.NewGuid();

    public required string Channel { get; init; } = "";
    public required string Text { get; init; } = "";
    public required IList<string> Recipients { get; init; } = [];
    
    public NotificationStatus Status { get; set; }
    
    public DateTime CreatedAt { get; init; }
}