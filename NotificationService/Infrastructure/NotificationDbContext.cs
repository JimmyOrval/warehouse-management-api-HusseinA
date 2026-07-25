using Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure;

public class NotificationDbContext(DbContextOptions<NotificationDbContext> options)
    : DbContext(options)
{
    public DbSet<Notification> Notifications => Set<Notification>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        
        builder.Entity<Notification>()
            .HasIndex(n => n.EventId)
            .IsUnique();

        builder.Entity<Notification>()
            .Property(n => n.Type)
            .HasConversion<string>();
        
        builder.Entity<Notification>()
            .Property(n => n.Severity)
            .HasConversion<string>();
        
        builder.Entity<Notification>()
            .Property(n => n.RelatedEntity)
            .HasConversion<string>();
        
        builder.Entity<Notification>()
            .Property(n => n.Status)
            .HasConversion<string>();
    }
}