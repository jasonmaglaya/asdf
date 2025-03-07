using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Remy.Gambit.Core.Caching;

namespace Remy.Gambit.Api.Caching;

public class MemoryCacheService(IMemoryCache cache, IConfiguration config) : ICacheService
{
    public bool TryGetValue<TItem>(object key, out TItem? value)
    {
        if (cache.TryGetValue(key, out object? value2))
        {
            if (value2 == null)
            {
                value = default;
                return true;
            }

            if (value2 is TItem val)
            {
                value = val;
                return true;
            }
        }

        value = default;
        return false;
    }

    public TItem Set<TItem>(object key, TItem value)
    {
        var options = new MemoryCacheEntryOptions()
                    .SetAbsoluteExpiration(TimeSpan.FromHours(config.GetValue<int>("Caching:AbsoluteExpirationInHours", 24)));

        using ICacheEntry cacheEntry = cache.CreateEntry(key);
        if (options != null)
        {
            cacheEntry.SetOptions(options);
        }

        cacheEntry.Value = value;
        return value;        
    }

    public TItem Set<TItem>(object key, TItem value, MemoryCacheEntryOptions options)
    {
        using ICacheEntry cacheEntry = cache.CreateEntry(key);
        if (options != null)
        {
            cacheEntry.SetOptions(options);
        }

        cacheEntry.Value = value;
        return value;
    }
}
