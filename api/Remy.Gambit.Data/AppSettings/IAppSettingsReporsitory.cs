using Remy.Gambit.Models;

namespace Remy.Gambit.Data.Features
{
    public interface IAppSettingsRepository
    {
        Task<IEnumerable<AppSetting>> GetAppSettingsAsync(CancellationToken token);

        Task<T> GetAppSettingValueAsync<T>(string key, CancellationToken token);
    }
}
