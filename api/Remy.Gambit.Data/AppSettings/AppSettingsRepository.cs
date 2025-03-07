using Remy.Gambit.Core.Caching;
using Remy.Gambit.Data.Features;
using Remy.Gambit.Data.Features.DataQueries;
using Remy.Gambit.Models;

namespace Remy.Gambit.Data.AppSettings;

public class AppSettingsRepository(IGambitDbClient gambitDbClient, ICacheService cache) : IAppSettingsRepository
{
    private const string APP_SETTINGS = "AppSettings";
    private static readonly SemaphoreSlim _semaphoreSlim = new(1, 1);

    public async Task<IEnumerable<AppSetting>> GetAppSettingsAsync(CancellationToken token)
    {
        cache.TryGetValue(APP_SETTINGS, out IEnumerable<AppSetting>? appSettings);

        if (appSettings is not null && appSettings.Any())
        {
            return appSettings;
        }

        try
        {
            await _semaphoreSlim.WaitAsync(token);

            cache.TryGetValue(APP_SETTINGS, out appSettings);

            if (appSettings is not null && appSettings.Any())
            {
                return appSettings;
            }

            var query = new GetAppSettingsQuery();

            appSettings = await gambitDbClient.GetCollectionAsync<AppSetting>(query, token);

            if (appSettings is not null && appSettings.Any())
            {
                cache.Set(APP_SETTINGS, appSettings);
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

        return appSettings ?? [];
    }

    public async Task<T> GetAppSettingValueAsync<T>(string key, CancellationToken token)
    {
        var appSettings = await GetAppSettingsAsync(token);

        var setting = appSettings.FirstOrDefault(x => x.SettingKey == key);

        return (T)Convert.ChangeType(setting!.Value, typeof(T)); ;
    }
}
