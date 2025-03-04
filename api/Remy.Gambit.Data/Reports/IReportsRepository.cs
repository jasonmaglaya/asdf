using Remy.Gambit.Models.Reports;

namespace Remy.Gambit.Data.Reports;

public interface IReportsRepository
{
    Task<IEnumerable<EventsReportItem>> GetEventsReportAsync(CancellationToken token);

    Task<IEnumerable<EventSummaryItem>> GetEventSummaryAsync(Guid eventId, CancellationToken token);

    Task<IEnumerable<PlayerEventSummaryItem>> GetPlayerEventSummaryAsync(Guid eventId, Guid userId, CancellationToken token);

    Task<IEnumerable<MatchSummaryItem>> GetMatchSummaryAsync(Guid matchId, CancellationToken token);

    Task<IEnumerable<PlayerBetSummaryItem>> GetPlayerBetSummaryAsync(Guid matchId, Guid userId, CancellationToken token);
}
