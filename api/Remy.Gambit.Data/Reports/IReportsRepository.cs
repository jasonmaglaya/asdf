using Remy.Gambit.Models.Reports;

namespace Remy.Gambit.Data.Reports;

public interface IReportsRepository
{
    Task<IEnumerable<EventReportItem>> GetEventsReportAsync(CancellationToken token);

    Task<IEnumerable<EventSummaryItem>> GetEventSummaryAsync(Guid eventId, Guid userId, CancellationToken token);    
}
