using Microsoft.Extensions.Caching.Memory;
using Remy.Gambit.Data.Features.DataQueries;
using Remy.Gambit.Models;

namespace Remy.Gambit.Data.Features;

public class FeaturesRepository : IFeaturesRepository
{
    private readonly IGambitDbClient _gambitDbClient;
    private readonly IMemoryCache _cache;
    private const string ROLE_FEATURES = "RoleFeatures";
    private const int CACHE_EXPIRY_IN_HOUR = 1;
    private static SemaphoreSlim _semaphoreSlim = new(1, 1);

    public FeaturesRepository(IGambitDbClient gambitDbClient, IMemoryCache cache)
    {
        _gambitDbClient = gambitDbClient;
        _cache = cache;
    }

    private async Task<IEnumerable<RoleFeature>> GetAllRoleFeaturesAsync(CancellationToken token)
    {
        _cache.TryGetValue(ROLE_FEATURES, out IEnumerable<RoleFeature>? roleFeatures);

        if (roleFeatures is not null && roleFeatures.Any())
        {
            return roleFeatures;
        }

        try
        {
            await _semaphoreSlim.WaitAsync(token);

            _cache.TryGetValue(ROLE_FEATURES, out roleFeatures);

            if (roleFeatures is not null && roleFeatures.Any())
            {
                return roleFeatures;
            }

            var query = new GetAllRoleFeaturesQuery();

            roleFeatures = await _gambitDbClient.GetCollectionAsync<RoleFeature>(query, token);

            if (roleFeatures is not null && roleFeatures.Any())
            {
                var options = new MemoryCacheEntryOptions()
                    .SetAbsoluteExpiration(TimeSpan.FromHours(CACHE_EXPIRY_IN_HOUR));

                _cache.Set(ROLE_FEATURES, roleFeatures, options);
            }
        }
        catch
        {
            throw;
        }
        finally
        {
            _semaphoreSlim.Release();
        }

        return roleFeatures ?? new List<RoleFeature>();
    }

    public async Task<IEnumerable<string>> GetFeaturesByRoleAsync(string role, CancellationToken token)
    {
        var features = await GetAllRoleFeaturesAsync(token);

        return features.Where(x => x.Role == role).Select(x => x.Feature);
    }
}
