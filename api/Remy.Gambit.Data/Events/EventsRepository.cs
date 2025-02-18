using Remy.Gambit.Core.Generics;
using Remy.Gambit.Data.Events.DataQueries;
using Remy.Gambit.Models;

namespace Remy.Gambit.Data.Events;

public class EventsRepository(IGambitDbClient gambitDbClient) : IEventsRepository
{
    private readonly IGambitDbClient _gambitDbClient = gambitDbClient;

    public async Task<(Match?, IEnumerable<Team>, IEnumerable<Team>)> GetCurrentMatchAsync(Guid id, CancellationToken token)
    {
        var query = new GetCurrentMatchQuery(id);

        var (FirstResult, SecondResult, ThirdResult) =  await _gambitDbClient.GetMultipleAsync<Match, Team, Team>(query, token);
        
        return (FirstResult.FirstOrDefault(), SecondResult, ThirdResult);
    }

    public async Task<Event> GetEventByIdAsync(Guid id, CancellationToken token)
    {
        var query = new GetEventQuery(id);

        var (firstResult, secondResult) =  await _gambitDbClient.GetMultipleAsync<Event, Team>(query, token);

        var @event = firstResult.FirstOrDefault();
        if(@event is not null)
        {
            @event.Teams = secondResult;
        }

        return @event!;
    }

    public async Task<PaginatedList<Event>> GetEventsAsync(string? status, int pageNumber, int pageSize, CancellationToken token)
    {
        var query = new GetEventsQuery(status, pageNumber, pageSize);

        var (events, total) = await _gambitDbClient.GetMultipleAsync<Event, int>(query, token);

        return new PaginatedList<Event> { List = events, PageSize = pageSize, TotalItems = total.FirstOrDefault() };
    }

    public async Task<Guid> AddMatchAsync(Guid eventId, CancellationToken token, int? number = null, string? description = null)
    {
        var query = new AddMatchQuery(eventId, number, description);

        return await _gambitDbClient.ExecuteScalarAsync<Guid>(query, token);
    }

    public async Task<Guid> AddEventAsync(Event @event, CancellationToken token)
    {
        var query = new AddEventQuery(@event);

        return await _gambitDbClient.ExecuteScalarAsync<Guid>(query, token);
    }

    public async Task<bool> UpdateEventAsync(Event @event, CancellationToken token)
    {
        var query = new UpdateEventQuery(@event);

        return await _gambitDbClient.ExecuteAsync(query, token) > 0;
    }

    public async Task<bool> UpdateEventStatusAsync(Guid eventId, string status, CancellationToken token)
    {
        var query = new UpdateEventStatusQuery(eventId, status);
        return await _gambitDbClient.ExecuteAsync(query, token) > 0;
    }

    public async Task<IEnumerable<Winner>> GetEventWinnersAsync(Guid eventId, CancellationToken token)
    {
        var query = new GetEventWinnersQuery(eventId);
        return await _gambitDbClient.GetCollectionAsync<Winner>(query, token);
    }
}
