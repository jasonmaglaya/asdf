using Remy.Gambit.Core.Data;

namespace Remy.Gambit.Data.Matches.DataQueries;

public class GetMatchQuery : DataQuery
{
    private readonly string _query = @"
SELECT *
FROM Matches
WHERE Id = @Id

SELECT T.*
FROM Teams T
    JOIN Matches M ON T.EventId = M.EventId
WHERE M.Id = @Id

SELECT TeamCode as Code
FROM MatchWinners
WHERE MatchId = @Id
    AND IsDeleted = 0
";
    public GetMatchQuery(Guid id)
    {
        CmdText = _query;

        Parameters.Add("Id", id);
    }
}
