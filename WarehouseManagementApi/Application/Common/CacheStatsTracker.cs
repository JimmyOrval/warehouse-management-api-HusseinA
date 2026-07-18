using System.Collections.Concurrent;
using Application.ViewModels;

namespace Application.Common;

public interface ICacheStatsTracker
{
    void RecordHit();
    void RecordMiss();
    void RecordSet(string key);
    void RecordRemoval(string key);
    CacheStatsViewModel GetStats();
}

public class CacheStatsTracker : ICacheStatsTracker
{
    private readonly ConcurrentDictionary<string, byte> _keys = new();
    private int _hits;
    private int _misses;
    private DateTime? _lastRefreshed;
    
    public void RecordHit()
    {
        Interlocked.Increment(ref _hits);
    }

    public void RecordMiss()
    {
        Interlocked.Increment(ref _misses);
    }

    public void RecordSet(string key)
    {
        _keys[key] = 0;
        _lastRefreshed = DateTime.UtcNow;
    }

    public void RecordRemoval(string key)
    {
        _keys.TryRemove(key, out _);
    }

    public CacheStatsViewModel GetStats()
    {
        return new CacheStatsViewModel(
            _keys.Keys.ToList(), _hits, _misses, _lastRefreshed);
    }
}