using Remy.Gambit.Core.Generics;
using Remy.Gambit.Models;

namespace Remy.Gambit.Data.Matches;

public interface IMatchesRepository
{
    Task<Guid> AddMatchAsync(Guid eventId, Match match, CancellationToken token);

    Task<Match?> GetMatchAsync(Guid id, CancellationToken token);

    Task<IEnumerable<Winner>> GetMatchWinnersAsync(Guid matchId, CancellationToken token);

    Task<(IEnumerable<TotalBet>, decimal)> GetTotalBetsAsync(Guid id, CancellationToken token);

    Task<IEnumerable<TotalBet>> GetBetsByUserIdAsync(Guid id, Guid userId, CancellationToken token);

    Task<decimal?> AddBetAsync(Guid userId, Guid matchId, string teamCode, decimal amount, string ipAddress, CancellationToken token);

    Task<bool> UpdateStatusAsync(Guid matchId, string status, CancellationToken token);

    Task<bool> DeclareWinnerAsync(Guid matchId, IEnumerable<string> teamCodes, Guid declaredBy, string ipAddress, CancellationToken token);

    Task<bool> ReDeclareWinnerAsync(Guid matchId, IEnumerable<string> teamCodes, Guid userId, string ipAddress, CancellationToken token);

    Task<bool> CancelMatchAsync(Guid matchId, Guid cancelledBy, string ipAddress, CancellationToken token);

    Task<PaginatedList<Match>> GetMatchesAsync(Guid eventId, int pageNumber, int pageSize, CancellationToken token);
}
