using Remy.Gambit.Data.Credits.DataQueries;
using Remy.Gambit.Models;

namespace Remy.Gambit.Data.Credits;

public class CreditsRepository(IGambitDbClient gambitDbClient) : ICreditsRepository
{
    private readonly IGambitDbClient _gambitDbClient = gambitDbClient;

    public async Task<bool> CashInAsync(Credit credit, string notes, CancellationToken cancellationToken)
    {
        var query = new CashInQuery(credit, notes);

        return await _gambitDbClient.ExecuteAsync(query, cancellationToken) > 0;
    }

    public async Task<bool> CashOutAsync(Credit credit, string notes, CancellationToken cancellationToken)
    {
        var query = new CashOutQuery(credit);

        return await _gambitDbClient.ExecuteAsync(query, cancellationToken) > 0;
    }
}
