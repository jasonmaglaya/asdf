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

UPDATE MatchWinners SET
    IsDeleted = 1
WHERE MatchId = @MatchId;

DECLARE @GroupTransactionId UNIQUEIDENTIFIER = NEWID()

INSERT INTO Credits (UserId, Amount, TransactionDate, TransactionType, TransactedBy, BetId, Notes, GroupTransactionId)	
SELECT C.UserId, C.Amount * -1 Amount, GETUTCDATE(), 'Betting-Rollback', @CancelledBy, C.BetId, 'REVERSAL (CANCELLED)', @GroupTransactionId
FROM Credits C
JOIN Bets B
	ON C.BetId = B.Id	
WHERE B.MatchId = @MatchId
";

    public CancelMatchQuery(Guid matchId, Guid cancelledBy, string ipAddress)
    {
        CmdText = _query;

        Parameters.Add("MatchId", matchId);
        Parameters.Add("CancelledBy", cancelledBy);
        Parameters.Add("IpAddress", ipAddress);
    }
}
