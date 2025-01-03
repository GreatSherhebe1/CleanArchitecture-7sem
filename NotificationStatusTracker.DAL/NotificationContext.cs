using Microsoft.EntityFrameworkCore;
using NotificationStatusTracker.DAL.Models;

namespace NotificationStatusTracker.DAL;

public class NotificationContext(DbContextOptions<NotificationContext> options) : DbContext(options)
{
    public DbSet<Notification> Notifications { get; set; }
}