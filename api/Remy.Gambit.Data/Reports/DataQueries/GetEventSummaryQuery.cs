using Remy.Gambit.Core.Data;

namespace Remy.Gambit.Data.Reports.DataQueries;

public class GetEventSummaryQuery : DataQuery
{
    private readonly string _query = @"
SELECT
	M.Id MatchId,
	M.Number,
	CASE WHEN COALESCE(MW.Winners, M.Status) = 'Cancelled' THEN 'C' ELSE MW.Winners END Winners,
	CASE WHEN COALESCE(MW.Winners, M.Status) = 'Cancelled' THEN M.CancelDate ELSE MW.DeclareDate END DeclareDate,
	DB.Username DeclaredBy,
	COALESCE(MW.IpAddress, M.IpAddress) IpAddress,
	SUM(
		CASE 
			WHEN B.TeamCode = 'D' THEN 0 			
			ELSE COALESCE(B.Amount, 0) 
		END
	) TotalBets,
	SUM(
		CASE 
			WHEN B.TeamCode = 'D' THEN 0 
			WHEN B.TeamCode <> 'D' AND (MW.Winners = 'D' OR MW.Winners IS NULL) THEN 0
			ELSE COALESCE(B.Amount, 0) 
		END * E.Commission
	) Commission,
	SUM(
		CASE 
			WHEN B.TeamCode = 'D' THEN COALESCE(B.Amount, 0)
			ELSE 0
		END
	) TotalDraw,
	SUM(
		COALESCE(B.Amount, 0) * 
		CASE 
			WHEN B.TeamCode = 'D' AND MW.Winners = 'D' THEN E.DrawMultiplier - 1
			ELSE 0
		END - (
			COALESCE(B.Amount, 0) * 
			CASE 
				WHEN B.TeamCode = 'D' AND MW.Winners = 'D' THEN E.DrawMultiplier - 1
				ELSE 0
			END * E.Commission
		)
	) TotalDrawPayout,
	SUM(
		CASE
			WHEN B.TeamCode = 'D' AND MW.Winners IS NOT NULL THEN COALESCE(B.Amount, 0)
			ELSE 0 
		END
	) - 
	SUM(
		COALESCE(B.Amount, 0) * 
		CASE 
			WHEN B.TeamCode = 'D' AND MW.Winners = 'D' THEN E.DrawMultiplier - 1
			ELSE 0
		END - (
			COALESCE(B.Amount, 0) * 
			CASE 
				WHEN B.TeamCode = 'D' AND MW.Winners = 'D' THEN E.DrawMultiplier - 1
				ELSE 0
			END * E.Commission
		)
	) DrawNet
FROM [Events] E
	LEFT JOIN Matches M
		ON M.EventId = E.Id
	LEFT JOIN Bets B
		ON B.MatchId = M.Id
	LEFT JOIN (
		SELECT MatchId, STRING_AGG(TeamCode, ', ') AS Winners, DeclareDate, DeclaredBy, IpAddress
		FROM MatchWinners		
		GROUP BY MatchId, DeclareId, DeclareDate, DeclaredBy, IpAddress
	) MW
		ON B.MatchId = MW.MatchId
	LEFT JOIN Users DB
		ON COALESCE(MW.DeclaredBy, M.CancelledBy) = DB.Id
WHERE E.Id = @EventId
	AND E.Status IN ('Active', 'Closed')
	AND M.Status <> 'New'
GROUP BY M.Id, M.Number, MW.Winners, E.Commission, E.DrawMultiplier, MW.DeclareDate, DB.Username, MW.IpAddress, M.Status, M.CancelDate, M.IpAddress
ORDER BY M.Number, MW.DeclareDate
";

    public GetEventSummaryQuery(Guid eventId)
    {
        CmdText = _query;

		Parameters.Add("EventId", eventId);
    }

}
