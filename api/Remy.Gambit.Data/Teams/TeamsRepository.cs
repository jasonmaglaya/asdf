using Remy.Gambit.Data.Teams.DataQueries;
using Remy.Gambit.Models;

namespace Remy.Gambit.Data.Teams;

public class TeamsRepository(IGambitDbClient gambitDbClient) : ITeamsRepository
{
    private readonly IGambitDbClient _gambitDbClient = gambitDbClient;

    public async Task<bool> AddTeamAsync(Guid eventId, Team team, CancellationToken token)
    {
        var query = new AddTeamQuery(eventId, team);
        
        return await _gambitDbClient.ExecuteAsync(query, token) > 0;
    }

    public async Task<int> DeleteTeamsByEventIdAsync(Guid eventId, CancellationToken token)
    {
        var query = new DeleteTeamsByEventId(eventId);

        return await _gambitDbClient.ExecuteAsync(query, token);
    }
}
