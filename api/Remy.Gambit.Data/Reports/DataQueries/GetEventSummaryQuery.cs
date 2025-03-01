using Remy.Gambit.Core.Data;

namespace Remy.Gambit.Data.Reports.DataQueries;

public class GetEventSummaryQuery : DataQuery
{
    private readonly string _query = @"
SELECT
	M.Id MatchId,
	M.Number,
	MW.Winners,
	SUM (
		CASE WHEN B.TeamCode = 'D' THEN 0 ELSE B.Amount END
	) TotalBets,
	SUM (
		CASE WHEN B.TeamCode = 'D' THEN B.Amount ELSE 0 END
	) TotalDraw,
	SUM (
		CASE WHEN B.TeamCode = 'D' AND MW.Winners = 'D' THEN B.Amount ELSE 0 END * (1 - E.Commission)
	) * (E.DrawMultiplier - 1) TotalDrawPayout,
	CASE WHEN MW.Winners = 'D' THEN
		(SUM (
			CASE WHEN B.TeamCode = 'D' THEN B.Amount ELSE 0 END
		) * (E.DrawMultiplier - 1)) * E.Commission
	ELSE
		SUM (
			CASE WHEN B.TeamCode = 'D' OR MW.Winners IN ('C', 'D') THEN 0 ELSE B.Amount END
		) * E.Commission 
	END Commission,
	SUM (
		CASE WHEN B.TeamCode = 'D' AND MW.Winners <> 'C' THEN B.Amount ELSE 0 END
	) -
	SUM (
		CASE WHEN B.TeamCode = 'D' AND MW.Winners = 'D' THEN B.Amount ELSE 0 END * (1 - E.Commission)
	) * (E.DrawMultiplier - 1) DrawNet,
	MW.DeclareDate,
	DB.Username DeclaredBy,
	MW.IpAddress
FROM Bets B
	LEFT JOIN (
		SELECT MatchId, STRING_AGG(TeamCode, ',') AS Winners, DeclareDate, DeclaredBy, MTW.IpAddress, IsDeleted
		FROM MatchWinners MTW
			JOIN Matches MT
				ON MTW.MatchId = MT.Id
			JOIN [Events] EV
				ON MT.EventId = EV.Id
		WHERE EV.Id = @EventId
		GROUP BY MatchId, DeclareId, DeclareDate, DeclaredBy, MTW.IpAddress, IsDeleted
		UNION
		SELECT MTC.Id, 'C', CancelDate, CancelledBy, IpAddress, 0
		FROM Matches MTC
			JOIN [Events] EVT
				ON MTC.EventId = EVT.Id
		WHERE EVT.Id = @EventId
			AND MTC.Status = 'Cancelled'
	) MW
		ON B.MatchId = MW.MatchId
	LEFT JOIN Users DB
		ON MW.DeclaredBy = DB.Id
	JOIN Matches M
		ON B.MatchId = M.Id
	JOIN [Events] E
		ON M.EventId = E.Id	
WHERE E.Id = @EventId
	AND E.Status IN ('Active', 'Closed')
	AND M.Status NOT IN ('New', 'Open')
GROUP BY
	M.Id,
	M.Number,
	MW.Winners,
	MW.DeclareDate,
	MW.IsDeleted,
	E.Commission,
	E.DrawMultiplier,
	MW.DeclareDate,
	DB.Username,
	MW.IpAddress
ORDER BY
	M.Number, 
	MW.DeclareDate
";

    public GetEventSummaryQuery(Guid eventId)
    {
        CmdText = _query;

		Parameters.Add("EventId", eventId);
    }

}
