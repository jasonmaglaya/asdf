using Remy.Gambit.Core.Data;

namespace Remy.Gambit.Data.Matches.DataQueries;

public class GetMatchesQuery : DataQuery
{
    private readonly string _query = @"
DECLARE @pn INT
SET @pn = COALESCE(@PageNumber, 1)

DECLARE @ps INT
SET @ps = COALESCE(@PageSize, 20)

SELECT M.Id, M.EventId, M.Number, M.Status, M.Description, M.Sequence, STRING_AGG(MW.TeamCode, ', ') WinnerCode
FROM Matches M
	LEFT JOIN MatchWinners MW
		ON M.Id = MW.MatchId
WHERE EventId = @EventId
    AND MW.IsDeleted = 0
GROUP BY M.Id, M.EventId, M.Number, M.Status, M.Description, M.Sequence
ORDER BY M.Sequence DESC
OFFSET (@pn-1)*@ps ROWS
FETCH NEXT @ps ROWS ONLY

SELECT COUNT(*) Count
FROM Matches M
	LEFT JOIN MatchWinners MW
		ON M.Id = MW.MatchId            
WHERE EventId = @EventId
    AND MW.IsDeleted = 0
GROUP BY M.Id, M.EventId, M.Number, M.Status, M.Description, M.Sequence
";

    public GetMatchesQuery(Guid eventId, int? pageNumber, int? pageSize)
    {
        Parameters.Add("@EventId", eventId);
        Parameters.Add("@PageNumber", pageNumber);
        Parameters.Add("@PageSize", pageSize);

        CmdText = _query;
    }
}