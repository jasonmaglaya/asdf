using Remy.Gambit.Core.Data;

namespace Remy.Gambit.Data.Reports.DataQueries;

public class GetMatchSummaryQuery : DataQuery
{
    private readonly string _query = @"
SELECT
	U.Id UserId,
	U.Username,
	SUM(B.Amount) Amount
FROM Bets B
	JOIN Users U
		ON B.UserId = U.Id
WHERE MatchId = @MatchId
GROUP BY
	U.Id,
	U.Username
ORDER BY Amount DESC
";

    public GetMatchSummaryQuery(Guid matchId)
    {
        CmdText = _query;

		Parameters.Add("MatchId", matchId);
    }
}
