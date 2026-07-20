namespace Application.ViewModels;

public record CacheStatsViewModel(
    List<string> CachedKeys,
    int HitCount,
    int MissCount,
    DateTime? LastRefreshedAt);