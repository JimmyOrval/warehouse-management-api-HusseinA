namespace Application.ViewModels;

public record UnreadNotificationCountViewModel(
    int? Count,
    bool IsAvailable);