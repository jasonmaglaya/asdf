using Remy.Gambit.Core.Data;

namespace Remy.Gambit.Data.Matches.DataQueries;

public class ProcessBetsQuery : DataQuery
{
    private readonly string _query = @"
DECLARE @Commission FLOAT
DECLARE @DrawMultiplier FLOAT
SELECT
	@Commission = E.Commission,
	@DrawMultiplier = E.DrawMultiplier
FROM Matches M
	JOIN Events E
		ON M.EventId = E.Id
WHERE M.Id = @MatchId

DECLARE @TotalBetsAfterComm MONEY
SELECT @TotalBetsAfterComm = COALESCE(SUM(B.Amount),0) * (1 - @Commission)
FROM Matches M
	JOIN Teams T
		ON M.EventId = T.EventId
	LEFT JOIN Bets B
		ON T.EventId = M.EventId
		AND T.Code = B.TeamCode		
		AND B.MatchId = M.Id
WHERE M.Id = @MatchId

BEGIN TRANSACTION;

BEGIN TRY

	DECLARE @GroupTransactionId UNIQUEIDENTIFIER = NEWID()

	INSERT INTO Credits (UserId, Amount, TransactionDate, TransactionType, TransactedBy, BetId, GroupTransactionId)
	SELECT
		B.UserId, 
		CASE 
			WHEN B.TeamCode = W.TeamCode
				THEN
					CASE WHEN W.TeamCode = 'D'
						THEN (B.Amount * @DrawMultiplier) * (1 - @Commission)
					ELSE
						B.Amount * (O.Odds - 1)
					END
			ELSE
				B.Amount * -1
		END,
		GETUTCDATE(), 'Betting', 'System', B.Id, @GroupTransactionId
	FROM Bets B
		LEFT JOIN MatchWinners W
			ON B.TeamCode = W.TeamCode
				AND B.MatchId = W.MatchId
		LEFT JOIN (
			SELECT T.Code, @TotalBetsAfterComm / COALESCE(SUM(B.Amount),0) Odds
			FROM Matches M
				JOIN Teams T
					ON M.EventId = T.EventId
				LEFT JOIN Bets B
					ON T.EventId = M.EventId
					AND T.Code = B.TeamCode
					AND B.MatchId = M.Id
			WHERE M.Id = @MatchId
			GROUP BY T.Code
		) O 
			ON B.TeamCode = O.Code
		LEFT JOIN Credits T
			ON B.Id = T.BetId
	WHERE B.MatchId = @MatchId
		AND B.Status = 'Open'
		AND T.Id IS NULL
END TRY
BEGIN CATCH    
    IF @@TRANCOUNT > 0
        ROLLBACK TRANSACTION;
END CATCH;

IF @@TRANCOUNT > 0
    COMMIT TRANSACTION;
";

    public ProcessBetsQuery(Guid matchId)
    {
        CmdText = _query;

			Parameters.Add("MatchId", matchId);
    }
}
