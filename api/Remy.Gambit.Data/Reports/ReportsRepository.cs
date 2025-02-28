using Remy.Gambit.Data.Reports.DataQueries;
using Remy.Gambit.Models.Reports;

namespace Remy.Gambit.Data.Reports;

public class ReportsRepository(IGambitDbClient gambitDbClient) : IReportsRepository
{
    private readonly IGambitDbClient _gambitDbClient = gambitDbClient;

    public async Task<IEnumerable<EventReportItem>> GetEventsReportAsync(CancellationToken token)
    {
        var query = new GetEventsReportQuery();

        return await _gambitDbClient.GetCollectionAsync<EventReportItem>(query, token);
    }

    public async Task<IEnumerable<EventSummaryItem>> GetEventSummaryAsync(Guid eventId, Guid userId, CancellationToken token)
    {
        var query = new GetEventSummaryQuery(eventId, userId);

        return await _gambitDbClient.GetCollectionAsync<EventSummaryItem>(query, token);
    }
}
