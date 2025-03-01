using Remy.Gambit.Core.Data;

namespace Remy.Gambit.Data.Reports.DataQueries;

public class GetEventsReportQuery : DataQuery
{
    private readonly string _query = @"
SELECT
	E.Id EventId,
	E.Title,
	E.EventDate,
	COUNT(DISTINCT M.Id) Matches,
	E.Commission CommissionPercentage,
	SUM(
		CASE 
			WHEN B.TeamCode = 'D' THEN 0 
			WHEN B.TeamCode <> 'D' AND MW.Winners = 'D' THEN 0
			ELSE COALESCE(B.Amount, 0) 
		END
	) TotalBets,
	SUM(
		CASE 
			WHEN B.TeamCode = 'D' THEN 0 
			WHEN B.TeamCode <> 'D' AND MW.Winners = 'D' THEN 0
			ELSE COALESCE(B.Amount, 0) 
		END * E.Commission
	) Commission,
	E.DrawMultiplier,
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
			WHEN B.TeamCode = 'D' THEN COALESCE(B.Amount, 0)
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
			AND B.Status = 'Final'
	LEFT JOIN (
		SELECT MatchId, STRING_AGG(TeamCode, ', ') AS Winners
		FROM MatchWinners
		WHERE IsDeleted = 0
		GROUP BY MatchId, DeclareId
	) MW
		ON B.MatchId = MW.MatchId
WHERE E.Status IN ('Active', 'Closed')
	AND M.Status NOT IN ('New', 'Open')
GROUP BY E.Id, E.Title, M.EventId, E.EventDate, E.Commission, E.DrawMultiplier
ORDER BY E.EventDate DESC
";

    public GetEventsReportQuery()
    {
        CmdText = _query;
    }
}
