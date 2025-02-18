using Remy.Gambit.Models;

namespace Remy.Gambit.Data.ClientSecrets;

public interface IClientSecretsRepository
{
    ValueTask<ClientSecret?> GetClientSecretAsync(string id, CancellationToken token = default);
}
