namespace Application.ViewModels;

public record NotificationViewModel(
    string Id,
    string Type,
    string Title,
    string Message,
    string Severity,
    string Status,
    string RelatedEntityId,
    string RelatedEntity,
    DateTime CreatedAt,
    DateTime? ReadAt
);
