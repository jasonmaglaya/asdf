using Remy.Gambit.Core.Data;

namespace Remy.Gambit.Data.Matches.DataQueries;

public class DeclareWinnerQuery : DataQuery
{
    private readonly string _query = @"
INSERT INTO MatchWinners (MatchId, TeamCode)
VALUES (@MatchId, @TeamCodes)
";

    public DeclareWinnerQuery(Guid matchId, IEnumerable<string> teamCodes)
    {
			CmdText = _query;

			Parameters.Add("MatchId", matchId);
			Parameters.Add("TeamCodes", teamCodes);
    }
}
