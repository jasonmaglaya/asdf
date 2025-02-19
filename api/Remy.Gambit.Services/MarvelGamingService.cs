
using System.Text;
using System.Text.Json;

namespace Remy.Gambit.Services
{
    public class MarvelGamingService (IHttpClientFactory httpClientFactory) : IPartnerService
    {
        private readonly IHttpClientFactory _httpClientFactory = httpClientFactory;

        public async Task<string> GetBalanceAsync(string username, string userToken, CancellationToken cancellationToken = default)
        {
            var httpClient = _httpClientFactory.CreateClient("MarvelGaming");

            var payload = JsonSerializer.Serialize(new
            {
                type = "balance",
                token = userToken
            });

            var response = await httpClient.PostAsync("provider/fund/transfer", new StringContent(payload, Encoding.UTF8, "application/json"), cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                var res = await response.Content.ReadAsStringAsync(cancellationToken);

                throw new Exception($"Failed to get balance for user {username}.\nResponse:{res}");
            }

            return await response.Content.ReadAsStringAsync(cancellationToken);

            //var result = JsonSerializer.Deserialize<BalanceResponse>(content);

            //return result?.Balance ?? 0;
        }

        private class BalanceResponse
        {
            public string? Type { get; set; }
            public string? UserName { get; set; }
            public string? Status { get; set; }
            public string? Currency { get; set; }
            public decimal Balance { get; set; }
        }
    }
}
