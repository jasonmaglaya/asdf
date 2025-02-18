using Remy.Gambit.Core.Data;

namespace Remy.Gambit.Data.Events.DataQueries;

public class GetCurrentMatchQuery : DataQuery
{
    private readonly string _query = @"
DECLARE @CurrentMatchId UNIQUEIDENTIFIER

SELECT @CurrentMatchId = CurrentMatch
FROM Events
WHERE Id = @Id

SELECT *
FROM Matches
WHERE Id = @CurrentMatchId

SELECT *
FROM Teams
WHERE EventId = @Id

SELECT TeamCode as Code
FROM MatchWinners
WHERE MatchId = @CurrentMatchId
";

    public GetCurrentMatchQuery(Guid id)
    {
        CmdText = _query;

        Parameters.Add("Id", id);
    }
}
