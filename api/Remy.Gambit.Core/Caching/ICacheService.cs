namespace Remy.Gambit.Core.Caching;

public interface ICacheService
{
    bool TryGetValue<TItem>(object key, out TItem? value);

    TItem Set<TItem>(object key, TItem value);
}
