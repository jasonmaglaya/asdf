using Remy.Gambit.Core.Data;

namespace Remy.Gambit.Data.Matches.DataQueries;

public class CancelMatchQuery : DataQuery
{
    private readonly string _query = @"
UPDATE Matches SET
    Status = 'Cancelled',
    CancelDate = GETUTCDATE(),
    CancelledBy = @CancelledBy,
    IpAddress = @IpAddress
WHERE Id = @MatchId

UPDATE Bets SET
	Status = 'Cancelled'    
WHERE MatchId = @MatchId
";

    public CancelMatchQuery(Guid matchId, Guid cancelledBy, string ipAddress)
    {
        CmdText = _query;

        Parameters.Add("MatchId", matchId);
        Parameters.Add("CancelledBy", cancelledBy);
        Parameters.Add("IpAddress", ipAddress);
    }
}
