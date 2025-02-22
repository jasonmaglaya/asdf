using Remy.Gambit.Core.Generics;
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

    public async Task<PaginatedList<Credit>> GetHistoryAsync(Guid userId, int pageNumber, int pageSize, CancellationToken token)
    {
        var query = new GetHistoryQuery(userId, pageNumber, pageSize);

        var (history, total) = await _gambitDbClient.GetMultipleAsync<Credit, int>(query, token);

        return new PaginatedList<Credit> { List = history, PageSize = pageSize, TotalItems = total.FirstOrDefault() };
    }
}
