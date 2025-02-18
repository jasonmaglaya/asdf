using Microsoft.Extensions.Caching.Memory;
using Remy.Gambit.Data.Features.DataQueries;
using Remy.Gambit.Models;

namespace Remy.Gambit.Data.Features;

public class AppSettingsRepository : IAppSettingsRepository
{
    private readonly IGambitDbClient _gambitDbClient;
    private readonly IMemoryCache _cache;
    private const string APP_SETTINGS = "AppSettings";
    private const int CACHE_EXPIRY_IN_HOUR = 1;
    private static SemaphoreSlim _semaphoreSlim = new(1, 1);

    public AppSettingsRepository(IGambitDbClient gambitDbClient, IMemoryCache cache)
    {
        _gambitDbClient = gambitDbClient;
        _cache = cache;
    }

    public async Task<IEnumerable<AppSetting>> GetAppSettingsAsync(CancellationToken token)
    {
        _cache.TryGetValue(APP_SETTINGS, out IEnumerable<AppSetting>? appSettings);

        if (appSettings is not null && appSettings.Any())
        {
            return appSettings;
        }

        try
        {
            await _semaphoreSlim.WaitAsync(token);

            _cache.TryGetValue(APP_SETTINGS, out appSettings);

            if (appSettings is not null && appSettings.Any())
            {
                return appSettings;
            }

            var query = new GetAppSettingsQuery();

            appSettings = await _gambitDbClient.GetCollectionAsync<AppSetting>(query, token);

            if (appSettings is not null && appSettings.Any())
            {
                var options = new MemoryCacheEntryOptions()
                    .SetAbsoluteExpiration(TimeSpan.FromHours(CACHE_EXPIRY_IN_HOUR));

                _cache.Set(APP_SETTINGS, appSettings, options);
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

        return appSettings ?? new List<AppSetting>();
    }

    public async Task<T> GetAppSettingValueAsync<T>(string key, CancellationToken token)
    {
        var appSettings = await GetAppSettingsAsync(token);

        var setting = appSettings.FirstOrDefault(x => x.SettingKey == key);

        return (T)Convert.ChangeType(setting!.Value, typeof(T)); ;
    }
}
