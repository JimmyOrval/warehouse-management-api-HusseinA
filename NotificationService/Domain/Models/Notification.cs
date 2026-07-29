using System.ComponentModel.DataAnnotations;
using Domain.Enums;
using Domain.Exceptions;

namespace Domain.Models;

public class Notification
{
    [Required]
    [Length(36, 36)]
    public required string Id { get; set; } = Guid.NewGuid().ToString();
    
    [Required]
    [Length(36, 36)]
    public required string EventId { get; set; }
    
    [Required]
    public required NotificationType Type { get; set; }
    
    [Required]
    [StringLength(200)]
    public required string Title { get; set; }
    
    [Required]
    [StringLength(1000)]
    public required string Message { get; set; }
    
    [Required]
    public required NotificationSeverity Severity { get; set; }
    
    [Required]
    [Length(36, 36)]
    public required string RelatedEntityId { get; set; }
    
    [Required]
    public required RelatedEntity RelatedEntity { get; set; }

    [Required] public required NotificationStatus Status { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public DateTime? ReadAt { get; set; }
    
    
    public void MarkAsRead()
    {
        if(Status == NotificationStatus.Read)
            throw new BusinessRuleException("Notification already read");
        
        Status = NotificationStatus.Read;
        ReadAt = DateTime.UtcNow;
    }
    
    public void MarkAsFailed()
    {
        Status = NotificationStatus.Failed;
    }
}