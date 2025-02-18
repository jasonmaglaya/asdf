using Remy.Gambit.Core.Data;

namespace Remy.Gambit.Data.Matches.DataQueries;

public class GetTotalBetsQuery : DataQuery
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
GROUP BY T.Code
UNION
SELECT T.Code, COALESCE(SUM(B.Amount),0) Amount
FROM Matches M
	JOIN ( SELECT 'D' AS Code ) T
		ON M.EventId = M.EventId
	LEFT JOIN Bets B
		ON T.Code = B.TeamCode		
		AND B.MatchId = M.Id
WHERE M.Id = @MatchId
GROUP BY T.Code

SELECT E.Commission
FROM Matches M
	JOIN Events E
		ON M.EventId = E.Id
WHERE M.Id = @MatchId
";

    public GetTotalBetsQuery(Guid matchId)
    {
        CmdText = _query;

        Parameters.Add("MatchId", matchId);
    }
}
