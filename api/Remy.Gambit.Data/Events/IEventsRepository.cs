using Remy.Gambit.Core.Generics;
using Remy.Gambit.Models;

namespace Remy.Gambit.Data.Events;

public interface IEventsRepository
{
    Task<PaginatedList<Event>> GetEventsAsync(string? status, int pageNumber, int pageSize, bool includeNew, CancellationToken token);

    Task<Event> GetEventByIdAsync(Guid id, CancellationToken token);

    Task<(Match?, IEnumerable<Team>, IEnumerable<Team>)> GetCurrentMatchAsync(Guid id, CancellationToken token);

    Task<Guid> AddMatchAsync(Guid eventId, CancellationToken token, int? number = null, string? description = null);

    Task<Guid> AddEventAsync(Event @event, CancellationToken token);

    Task<bool> UpdateEventAsync(Event @event, CancellationToken token);

    Task<bool> UpdateEventStatusAsync(Guid eventId, string status, CancellationToken token);

    Task<IEnumerable<Winner>> GetEventWinnersAsync(Guid eventId, CancellationToken token);    
}
