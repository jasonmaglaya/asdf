
using Remy.Gambit.Data.ClientSecrets.DataQueries;
using Remy.Gambit.Models;

namespace Remy.Gambit.Data.ClientSecrets;

public class ClientSecretsRepository(IGambitDbClient gambitDbClient) : IClientSecretsRepository
{
    private readonly IGambitDbClient _gambitDbClient = gambitDbClient;

    public async ValueTask<ClientSecret?> GetClientSecretAsync(string id, CancellationToken token = default)
    {
        var query = new GetClientSecretQuery(id);

        return await _gambitDbClient.GetFirstOrDefaultAsync<ClientSecret>(query, token);
    }
}