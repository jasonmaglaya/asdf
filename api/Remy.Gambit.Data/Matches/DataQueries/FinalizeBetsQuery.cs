using Remy.Gambit.Core.Data;

namespace Remy.Gambit.Data.Matches.DataQueries;

public class FinalizeBetsQuery : DataQuery
{
    private readonly string _query = @"
UPDATE Bets SET
	Status = 'Final'
WHERE MatchId = @MatchId
";

    public FinalizeBetsQuery(Guid matchId)
    {
        CmdText = _query;

        Parameters.Add("MatchId", matchId);
    }
}
