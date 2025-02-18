using Remy.Gambit.Core.Data;

namespace Remy.Gambit.Data.Matches.DataQueries;

public class GetMatchWinnersQuery : DataQuery
{
    private readonly string _query = @"
SELECT TeamCode
FROM MatchWinners
WHERE MatchId = @MatchId
";
    public GetMatchWinnersQuery(Guid matchId)
    {
        CmdText = _query;

        Parameters.Add("MatchId", matchId);
    }
}
