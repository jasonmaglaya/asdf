using Remy.Gambit.Core.Data;

namespace Remy.Gambit.Data.Reports.DataQueries;

public class GetEventSummaryQuery : DataQuery
{
    private readonly string _query = @"
SELECT
	M.Number MatchNumber,
	B.TeamCode BetOn,
	B.Amount Bet,
	B.BetTimeStamp,
	B.OddsMultiplier Odds,
	COALESCE(MW.Winners, D.Winners) Winners,
	COALESCE(C.Amount, 0) GainLoss,
	COALESCE(C.TransactionDate, D.DeclareDate) GainLossDate,
	COALESCE(C.Notes, 'DRAW') Notes
FROM Bets B
	JOIN Matches M
		ON B.MatchId = M.Id
	LEFT JOIN Credits C
		ON B.Id = C.BetId
	LEFT JOIN (
		SELECT MatchId, DeclareId, STRING_AGG(TeamCode, ',') AS Winners
		FROM MatchWinners W
			JOIN Matches MT
				ON W.MatchId = MT.Id
		WHERE MT.EventId = @EventId
		GROUP BY MatchId, DeclareId
	) MW
		ON M.Id = MW.MatchId
			AND C.DeclareId = MW.DeclareId
	LEFT JOIN (
		SELECT MatchId, DeclareDate, STRING_AGG(TeamCode, ',') AS Winners
		FROM MatchWinners W
			JOIN Matches MT
				ON W.MatchId = MT.Id
		WHERE
			MT.EventId = @EventId
			AND W.IsDeleted = 0
		GROUP BY MatchId, DeclareId, DeclareDate
	) D
		ON M.Id = D.MatchId
WHERE
	M.EventId = @EventId
	AND B.UserId = @UserId
	AND B.Status = 'Final'
	AND COALESCE(C.TransactionType, 'Draw') <> 'Loading'	
ORDER BY M.Number, B.BetTimeStamp, C.TransactionDate, C.TransactionType DESC
";

    public GetEventSummaryQuery(Guid eventId, Guid userId)
    {
        CmdText = _query;

		Parameters.Add("EventId", eventId);
		Parameters.Add("UserId", userId);
    }
}
