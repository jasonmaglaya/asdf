using Remy.Gambit.Core.Data;

namespace Remy.Gambit.Data.Matches.DataQueries;

public class ReDeclareWinnerQuery : DataQuery
{
    private readonly string _query = @"
DELETE FROM MatchWinners
WHERE MatchId = @MatchId;

INSERT INTO MatchWinners (MatchId, TeamCode)
VALUES (@MatchId, @TeamCodes)
";

    public ReDeclareWinnerQuery(Guid matchId, IEnumerable<string> teamCodes)
    {
        CmdText = _query;

        Parameters.Add("MatchId", matchId);
        Parameters.Add("TeamCodes", teamCodes);
    }
}