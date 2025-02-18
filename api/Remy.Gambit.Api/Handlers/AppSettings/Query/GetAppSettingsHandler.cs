using Remy.Gambit.Api.Dto;
using Remy.Gambit.Api.Handlers.AppSettings.Query.Dto;
using Remy.Gambit.Core.Cqs;
using Remy.Gambit.Data.Features;

namespace Remy.Gambit.Api.Handlers.AppSettings.Query
{
    public class GetAppSettingsHandler(IAppSettingsRepository appSettingsRepository) : IQueryHandler<GetAppSettingsRequest, GetAppSettingsResult>
    {
        private readonly IAppSettingsRepository _appSettingsRepository = appSettingsRepository;

        public async ValueTask<GetAppSettingsResult> HandleAsync(GetAppSettingsRequest request, CancellationToken token = default)
        {
            var currency = await _appSettingsRepository.GetAppSettingValueAsync<string>(Constants.AppSettings.Currency, token);
            var locale = await _appSettingsRepository.GetAppSettingValueAsync<string>(Constants.AppSettings.Locale, token);

            var appSettings = new AppSetting
            {
                Currency = currency,
                Locale = locale
            };

            return new GetAppSettingsResult { IsSuccessful = true, Result = appSettings };
        }
    }
}
