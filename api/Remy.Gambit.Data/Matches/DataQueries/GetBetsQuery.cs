using Remy.Gambit.Core.Data;

namespace Remy.Gambit.Data.Matches.DataQueries;

public class GetBetsQuery : DataQuery
{
    private readonly string _query = @"
SELECT T.Code, COALESCE(SUM(B.Amount),0) Amount
FROM Matches M
	JOIN Teams T
		ON M.EventId = T.EventId
	LEFT JOIN Bets B
		ON T.EventId = M.EventId
		AND T.Code = B.TeamCode		
		AND B.MatchId = M.Id
WHERE M.Id = @MatchId
    AND B.UserId = @UserId
GROUP BY T.Code
UNION
SELECT T.Code, COALESCE(SUM(B.Amount),0) Amount
FROM Matches M
	JOIN (SELECT 'D' AS Code) T
		ON M.EventId = M.EventId
	LEFT JOIN Bets B
		ON T.Code = B.TeamCode		
		AND B.MatchId = M.Id
WHERE M.Id = @MatchId
    AND B.UserId = @UserId
GROUP BY T.Code
";

    public GetBetsQuery(Guid matchId, Guid userId)
    {
        CmdText = _query;

        Parameters.Add("MatchId", matchId);
        Parameters.Add("UserId", userId);
    }
}
