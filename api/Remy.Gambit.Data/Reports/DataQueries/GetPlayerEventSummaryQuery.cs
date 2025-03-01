using Remy.Gambit.Core.Data;

namespace Remy.Gambit.Data.Reports.DataQueries;

public class GetPlayerEventSummaryQuery : DataQuery
{
    private readonly string _query = @"
SELECT
	M.Number MatchNumber,
	B.TeamCode BetOn,
	B.Amount Bet,
	B.BetTimeStamp,
	COALESCE(B.OddsMultiplier, 0) Odds,
	COALESCE(COALESCE(MW.Winners, D.Winners), 'C') Winners,
	COALESCE(C.Amount, 0) GainLoss,
	COALESCE(COALESCE(C.TransactionDate, D.DeclareDate), M.CancelDate) GainLossDate,
	COALESCE(C.Notes, '') Notes
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
	AND B.Status <> 'Open'
	AND COALESCE(C.TransactionType, 'Draw') <> 'Loading'	
ORDER BY M.Number, B.BetTimeStamp, C.TransactionDate, C.TransactionType DESC
";

    public GetPlayerEventSummaryQuery(Guid eventId, Guid userId)
    {
        CmdText = _query;

		Parameters.Add("EventId", eventId);
		Parameters.Add("UserId", userId);
    }
}
