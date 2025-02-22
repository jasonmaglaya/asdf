using Remy.Gambit.Core.Data;

namespace Remy.Gambit.Data.Matches.DataQueries;

public class DeclareWinnerQuery : DataQuery
{
    private readonly string _query = @"
INSERT INTO MatchWinners (MatchId, TeamCode, DeclareId, DeclareDate, DeclaredBy, IsDeleted, IpAddress)
VALUES (@MatchId, @TeamCodes, @DeclareId, GETUTCDATE(), @DeclaredBy, 0, @IpAddress)
";

    public DeclareWinnerQuery(Guid matchId, IEnumerable<string> teamCodes, Guid declareId, Guid declaredBy, string ipAddress)
    {
	    CmdText = _query;

	    Parameters.Add("MatchId", matchId);
	    Parameters.Add("TeamCodes", teamCodes);
        Parameters.Add("DeclareId", declareId);
        Parameters.Add("DeclaredBy", declaredBy);
        Parameters.Add("IpAddress", ipAddress);
    }
}
