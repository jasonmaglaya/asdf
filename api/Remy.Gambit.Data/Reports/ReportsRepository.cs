using Remy.Gambit.Data.Reports.DataQueries;
using Remy.Gambit.Models.Reports;

namespace Remy.Gambit.Data.Reports;

public class ReportsRepository(IGambitDbClient gambitDbClient) : IReportsRepository
{
    private readonly IGambitDbClient _gambitDbClient = gambitDbClient;

    public async Task<IEnumerable<EventsReportItem>> GetEventsReportAsync(CancellationToken token)
    {
        var query = new GetEventsReportQuery();

        return await _gambitDbClient.GetCollectionAsync<EventsReportItem>(query, token);
    }

    public async Task<IEnumerable<EventSummaryItem>> GetEventSummaryAsync(Guid eventId, CancellationToken token)
    {
        var query = new GetEventSummaryQuery(eventId);

        return await _gambitDbClient.GetCollectionAsync<EventSummaryItem>(query, token);
    }

    public async Task<IEnumerable<PlayerEventSummaryItem>> GetPlayerEventSummaryAsync(Guid eventId, Guid userId, CancellationToken token)
    {
        var query = new GetPlayerEventSummaryQuery(eventId, userId);

        return await _gambitDbClient.GetCollectionAsync<PlayerEventSummaryItem>(query, token);
    }

    public async Task<IEnumerable<MatchSummaryItem>> GetMatchSummaryAsync(Guid matchId, CancellationToken token)
    {
        var query = new GetMatchSummaryQuery(matchId);

        return await _gambitDbClient.GetCollectionAsync<MatchSummaryItem>(query, token);
    }

    public async Task<IEnumerable<PlayerBetSummaryItem>> GetPlayerBetSummaryAsync(Guid matchId, Guid userId, CancellationToken token)
    {
        var query = new GetPlayerBetSummaryQuery(matchId, userId);

        return await _gambitDbClient.GetCollectionAsync<PlayerBetSummaryItem>(query, token);
    }
}
