using Remy.Gambit.Core.Data;

namespace Remy.Gambit.Data.Reports.DataQueries;

public class GetPlayerBetSummaryQuery : DataQuery
{
    private readonly string _query = @"
SELECT
	U.Id UserId,
	U.Username,
	B.Amount,
	B.TeamCode,
	B.BetTimeStamp,
	B.IpAddress
FROM Bets B
	JOIN Users U
		ON B.UserId = U.Id
WHERE
	MatchId = @MatchId
	AND UserId = @UserId
ORDER BY BetTimeStamp
";

    public GetPlayerBetSummaryQuery(Guid matchId, Guid userId)
    {
        CmdText = _query;

		Parameters.Add("MatchId", matchId);
		Parameters.Add("UserId", userId);
    }
}
