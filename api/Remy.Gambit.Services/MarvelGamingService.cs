
using Remy.Gambit.Models;
using System.Text;
using System.Text.Json;

namespace Remy.Gambit.Services
{
    public class MarvelGamingService (IHttpClientFactory httpClientFactory) : IPartnerService
    {
        private readonly IHttpClientFactory _httpClientFactory = httpClientFactory;

        public async Task<Balance> GetBalanceAsync(string partnerToken, CancellationToken cancellationToken = default)
        {
            var httpClient = _httpClientFactory.CreateClient("MarvelGaming");

            var payload = JsonSerializer.Serialize(new
            {
                type = "balance",
                token = partnerToken
            });

            var response = await httpClient.PostAsync("provider/fund/transfer", new StringContent(payload, Encoding.UTF8, "application/json"), cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                var res = await response.Content.ReadAsStringAsync(cancellationToken);

                throw new Exception($"Failed to get balance.\nResponse:{res}");
            }

            var content = await response.Content.ReadAsStringAsync(cancellationToken);

            var getBalanceResponse = JsonSerializer.Deserialize<GetBalanceResponse>(content);

            if(getBalanceResponse is null || getBalanceResponse.Status != "success")
            {
                return new Balance
                {
                    Amount = 0,
                    Currency = content
                };
            }

            return new Balance
            {
                Amount = getBalanceResponse.Balance,
                Currency = content
            };
        }

        private class GetBalanceResponse
        {
            public string? Type { get; set; }
            public string? UserName { get; set; }
            public string? Status { get; set; }
            public string? Currency { get; set; }
            public decimal Balance { get; set; }
        }
    }
}
