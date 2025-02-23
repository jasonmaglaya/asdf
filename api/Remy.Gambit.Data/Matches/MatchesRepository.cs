using Remy.Gambit.Core.Generics;
using Remy.Gambit.Data.Matches.DataQueries;
using Remy.Gambit.Models;

namespace Remy.Gambit.Data.Matches;

public class MatchesRepository(IGambitDbClient gambitDbClient) : IMatchesRepository
{
    private readonly IGambitDbClient _gambitDbClient = gambitDbClient;

    public async Task<Guid> AddMatchAsync(Guid eventId, Match match, CancellationToken token)
    {
        var query = new AddMatchQuery(eventId, match);

        return await _gambitDbClient.ExecuteScalarAsync<Guid>(query, token);
    }

    public async Task<decimal?> AddBetAsync(Guid userId, Guid matchId, string teamCode, decimal amount, string ipAddress, CancellationToken token)
    {
        var query = new AddBetQuery(userId, matchId, teamCode, amount, ipAddress);

        return await _gambitDbClient.ExecuteScalarAsync<decimal?>(query, token);
    }

    public async Task<bool> CancelMatchAsync(Guid matchId, CancellationToken token)
    {
        var query = new CancelMatchQuery(matchId);

        return await _gambitDbClient.ExecuteAsync(query, token) > 0;
    }

    public async Task<bool> DeclareWinnerAsync(Guid matchId, IEnumerable<string> teamCodes, Guid declaredBy, string ipAddress, CancellationToken token)
    {
        var declareId = Guid.NewGuid();

        var declareWinnerQuery = new DeclareWinnerQuery(matchId, teamCodes, declareId, declaredBy, ipAddress);
        await _gambitDbClient.ExecuteAsync(declareWinnerQuery, token);

        var processBetsQuery = new ProcessBetsQuery(matchId, declareId);
        await _gambitDbClient.ExecuteAsync(processBetsQuery, token);

        //var processAgentCommissionsQuery = new ProcessAgentCommissionsQuery(matchId);
        //var processAgentCommissionsTask = _gambitDbClient.ExecuteAsync(processAgentCommissionsQuery, token);

        //var processMACommissionsQuery = new ProcessMACommissions(matchId);
        //var processMACommissionsTask = _gambitDbClient.ExecuteAsync(processMACommissionsQuery, token);

        //var processSMACommissionsQuery = new ProcessSMACommissions(matchId);
        //var processSMACommissionsTask = _gambitDbClient.ExecuteAsync(processSMACommissionsQuery, token);

        //var processINCOCommissionsQuery = new ProcessINCOCommissions(matchId);
        //var processINCOCommissionsTask = _gambitDbClient.ExecuteAsync(processINCOCommissionsQuery, token);

        //await Task.WhenAll(
        //    processBetsTask.AsTask(), 
        //    processAgentCommissionsTask.AsTask(),
        //    processMACommissionsTask.AsTask(),
        //    processSMACommissionsTask.AsTask(),
        //    processINCOCommissionsTask.AsTask()
        //);

        var finalizeBetsQuery = new FinalizeBetsQuery(matchId);
        await _gambitDbClient.ExecuteAsync(finalizeBetsQuery, token);

        return true;
    }

    public async Task<bool> ReDeclareWinnerAsync(Guid matchId, IEnumerable<string> teamCodes, Guid userId, string ipAddress, CancellationToken token)
    {
        var declareId = Guid.NewGuid();

        var declareWinnerQuery = new ReDeclareWinnerQuery(matchId, teamCodes, declareId, userId, ipAddress);
        await _gambitDbClient.ExecuteAsync(declareWinnerQuery, token);

        var processBetsQuery = new ReverseBetsQuery(matchId, userId, declareId);
        await _gambitDbClient.ExecuteAsync(processBetsQuery, token);

        // Re-Process Commissions
        //var processAgentCommissionsQuery = new ProcessAgentCommissionsQuery(matchId);
        //var processAgentCommissionsTask = _gambitDbClient.ExecuteAsync(processAgentCommissionsQuery, token);

        //var processMACommissionsQuery = new ProcessMACommissions(matchId);
        //var processMACommissionsTask = _gambitDbClient.ExecuteAsync(processMACommissionsQuery, token);

        //var processSMACommissionsQuery = new ProcessSMACommissions(matchId);
        //var processSMACommissionsTask = _gambitDbClient.ExecuteAsync(processSMACommissionsQuery, token);

        //var processINCOCommissionsQuery = new ProcessINCOCommissions(matchId);
        //var processINCOCommissionsTask = _gambitDbClient.ExecuteAsync(processINCOCommissionsQuery, token);

        //await Task.WhenAll(
        //    processBetsTask.AsTask(), 
        //    processAgentCommissionsTask.AsTask(),
        //    processMACommissionsTask.AsTask(),
        //    processSMACommissionsTask.AsTask(),
        //    processINCOCommissionsTask.AsTask()
        //);

        return true;
    }

    public async Task<IEnumerable<TotalBet>> GetBetsAsync(Guid id, Guid userId, CancellationToken token)
    {
        var query = new GetBetsQuery(id, userId);

        return await _gambitDbClient.GetCollectionAsync<TotalBet>(query, token);
    }

    public async Task<Match> GetMatchAsync(Guid id, CancellationToken token)
    {
        var query = new GetMatchQuery(id);

        return await _gambitDbClient.GetFirstOrDefaultAsync<Match>(query, token);
    }

    public async Task<(IEnumerable<TotalBet>, decimal)> GetTotalBetsAsync(Guid id, CancellationToken token)
    {
        var query = new GetTotalBetsQuery(id);

        var (FirstResult, SecondResult) = await _gambitDbClient.GetMultipleAsync<TotalBet, decimal>(query, token);

        return (FirstResult, SecondResult.FirstOrDefault());
    }

    public async Task<bool> UpdateStatusAsync(Guid matchId, string status, CancellationToken token)
    {
        var query = new UpdateStatusQuery(matchId, status);

        return await _gambitDbClient.ExecuteAsync(query, token) > 0;
    }

    public async Task<PaginatedList<Match>> GetMatchesAsync(Guid eventId, int pageNumber, int pageSize, CancellationToken token)
    {
        var query = new GetMatchesQuery(eventId, pageNumber, pageSize);

        var (matches, total) = await _gambitDbClient.GetMultipleAsync<Match, int>(query, token);

        return new PaginatedList<Match> { List = matches, PageSize = pageSize, TotalItems = total.FirstOrDefault() };
    }

    public async Task<IEnumerable<Winner>> GetMatchWinnersAsync(Guid matchId, CancellationToken token)
    {
        var query = new GetMatchWinnersQuery(matchId);

        return await _gambitDbClient.GetCollectionAsync<Winner>(query, token);
    }
}
