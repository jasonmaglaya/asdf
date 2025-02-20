
using Remy.Gambit.Models;
using Remy.Gambit.Services.Dto;
using System.Text;
using System.Text.Json;

namespace Remy.Gambit.Services
{
    public class MarvelGamingService (IHttpClientFactory httpClientFactory) : IPartnerService
    {
        private readonly IHttpClientFactory _httpClientFactory = httpClientFactory;

        public async Task<Balance> GetBalanceAsync(GetBalanceRequest request, CancellationToken cancellationToken = default)
        {
            request.Type = "balance";

            var response = await CallApiAsync<GetBalanceResponse>(request, cancellationToken);

            if (response is null || response.Status != "success")
            {
                return new Balance
                {
                    Amount = 0
                };
            }

            return new Balance
            {
                Amount = response.Balance,
                Currency = response.Currency
            };
        }

        public async Task<CashInResult> CashInAsync(CashInRequest request, CancellationToken cancellationToken = default)
        {
            request.Type = "bet";

            var response = await CallApiAsync<CashInResponse>(request, cancellationToken);

            if (response is null || response.Status != "success")
            {
                return new CashInResult
                {
                    IsSuccessful = false,
                    Errors = [JsonSerializer.Serialize(response)]
                };
            }

            return new CashInResult
            {
                IsSuccessful = true,
                NewBalance = response.Balance,
                Currency = response.Currency
            };
        }

        public async Task<CashOutResult> CashOutAsync(CashOutRequest request, CancellationToken cancellationToken = default)
        {
            request.Type = "betResult";

            var response = await CallApiAsync<CashOutResponse>(request, cancellationToken);

            if (response is null || response.Status != "success")
            {
                return new CashOutResult
                {
                    IsSuccessful = false,
                    Errors = [JsonSerializer.Serialize(response)]
                };
            }

            return new CashOutResult
            {
                IsSuccessful = true,
                NewBalance = response.Balance,
                Currency = response.Currency
            };
        }

        
        private async Task<T?> CallApiAsync<T>(RequestBase request, CancellationToken cancellationToken)
        {
            var httpClient = _httpClientFactory.CreateClient("MarvelGaming");

            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                IncludeFields = true,
                WriteIndented = true,
            };

            var payload = JsonSerializer.Serialize(request, request.GetType(), options);

            var response = await httpClient.PostAsync("provider/fund/transfer", new StringContent(payload, Encoding.UTF8, "application/json"), cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                var res = await response.Content.ReadAsStringAsync(cancellationToken);

                throw new Exception($"Failed to calling the partner API.\nResponse:{res}");
            }

            var content = await response.Content.ReadAsStringAsync(cancellationToken);

            return JsonSerializer.Deserialize<T>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }
    }
}
