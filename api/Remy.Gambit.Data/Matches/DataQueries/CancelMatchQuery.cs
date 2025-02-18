using Remy.Gambit.Core.Data;

namespace Remy.Gambit.Data.Matches.DataQueries;

public class CancelMatchQuery : DataQuery
{
    private readonly string _query = @"
UPDATE Matches SET
    Status = 'Cancelled'
WHERE Id = @MatchId

UPDATE Bets SET
	Status = 'Cancelled'
WHERE MatchId = @MatchId
";

    public CancelMatchQuery(Guid matchId)
    {
        CmdText = _query;

        Parameters.Add("MatchId", matchId);
    }
}
