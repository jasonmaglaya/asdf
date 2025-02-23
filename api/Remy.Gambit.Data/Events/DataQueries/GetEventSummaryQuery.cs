using Remy.Gambit.Core.Data;

namespace Remy.Gambit.Data.Events.DataQueries;

public class GetEventSummaryQuery : DataQuery
{
    private readonly string _query = @"
SELECT
	M.Number MatchNumber, 
	B.TeamCode BetOn,
	B.Amount Bet,
	B.BetTimeStamp, 
	MW.Winners, 
	C.Amount GainLoss,
	C.TransactionDate GainLossDate,
	C.Notes
FROM Bets B
	JOIN Matches M
		ON B.MatchId = M.Id
	JOIN Credits C
		ON B.Id = C.BetId
	LEFT JOIN (
		SELECT MatchId, DeclareId, STRING_AGG(TeamCode, ', ') AS Winners
		FROM MatchWinners
		GROUP BY MatchId, DeclareId
	) MW
		ON M.Id = MW.MatchId
			AND C.DeclareId = MW.DeclareId
WHERE
	M.EventId = @EventId
	AND C.UserId = @UserId
	AND C.TransactionType IN ('Betting', 'Betting-Rollback')
ORDER BY C.TransactionDate, C.TransactionType DESC
";

    public GetEventSummaryQuery(Guid eventId, Guid userId)
    {
        CmdText = _query;

		Parameters.Add("EventId", eventId);
		Parameters.Add("UserId", userId);
    }
}
