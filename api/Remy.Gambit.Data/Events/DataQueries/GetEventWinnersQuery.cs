using Remy.Gambit.Core.Data;

namespace Remy.Gambit.Data.Events.DataQueries;

public class GetEventWinnersQuery : DataQuery
{
    private readonly string _query = @"
SELECT M.Number, COALESCE(MW.TeamCode, 'C') TeamCode
FROM Matches M
	LEFT JOIN MatchWinners MW
		ON M.Id = MW.MatchId
            AND MW.IsDeleted = 0
WHERE M.EventId = @EventId
	AND M.Status IN ('Declared','Completed', 'Cancelled')    
ORDER BY M.[Sequence]
";

    public GetEventWinnersQuery(Guid eventId)
    {
        Parameters.Add("@EventId", eventId);

        CmdText = _query;
    }
}