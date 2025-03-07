using Remy.Gambit.Core.Caching;
using Remy.Gambit.Data.Features.DataQueries;
using Remy.Gambit.Models;

namespace Remy.Gambit.Data.Features;

public class FeaturesRepository(IGambitDbClient gambitDbClient, ICacheService cache) : IFeaturesRepository
{
    private readonly IGambitDbClient _gambitDbClient = gambitDbClient;
    private const string ROLE_FEATURES = "RoleFeatures";
    private static readonly SemaphoreSlim _semaphoreSlim = new(1, 1);

    private async Task<IEnumerable<RoleFeature>> GetAllRoleFeaturesAsync(CancellationToken token)
    {
        cache.TryGetValue(ROLE_FEATURES, out IEnumerable<RoleFeature>? roleFeatures);

        if (roleFeatures is not null && roleFeatures.Any())
        {
            return roleFeatures;
        }

        try
        {
            await _semaphoreSlim.WaitAsync(token);

            cache.TryGetValue(ROLE_FEATURES, out roleFeatures);

            if (roleFeatures is not null && roleFeatures.Any())
            {
                return roleFeatures;
            }

            var query = new GetAllRoleFeaturesQuery();

            roleFeatures = await _gambitDbClient.GetCollectionAsync<RoleFeature>(query, token);

            if (roleFeatures is not null && roleFeatures.Any())
            {
                cache.Set(ROLE_FEATURES, roleFeatures);
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

        return roleFeatures ?? [];
    }

    public async Task<IEnumerable<string>> GetFeaturesByRoleAsync(string role, CancellationToken token)
    {
        var features = await GetAllRoleFeaturesAsync(token);

        return features.Where(x => x.Role == role).Select(x => x.Feature);
    }
}
