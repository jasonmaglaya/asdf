using Remy.Gambit.Models;

namespace Remy.Gambit.Data.Teams;

public interface ITeamsRepository
{
    Task<bool> AddTeamAsync(Guid eventId, Team team, CancellationToken token);
    Task<int> DeleteTeamsByEventIdAsync(Guid eventId, CancellationToken token);
}
