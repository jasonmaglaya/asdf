using Remy.Gambit.Core.Data;

namespace Remy.Gambit.Data.Matches.DataQueries;

public class ReverseBetsQuery : DataQuery
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

	INSERT INTO Credits (UserId, Amount, TransactionDate, TransactionType, TransactedBy, BetId, Notes, GroupTransactionId, DeclareId)
	SELECT C.UserId, C.Amount * -1 Amount, GETUTCDATE(), 'Betting-Rollback', @UserId, C.BetId, 'REVERSAL (RE-DECLARED)', @GroupTransactionId, @DeclareId
	FROM Credits C
		JOIN Bets B
			ON C.BetId = B.Id	
	WHERE B.MatchId = @matchId

	INSERT INTO Credits (UserId, Amount, TransactionDate, TransactionType, TransactedBy, BetId, Notes, GroupTransactionId, DeclareId, Odds)
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
		GETUTCDATE(), 
		'Betting', 
		@UserId, 
		B.Id,
		CASE WHEN (
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
			END
		) > 0 THEN 'WINNINGS' ELSE 'LOSSES' END,		
		@GroupTransactionId, 
		@DeclareId,
		O.Odds
	FROM Bets B
		LEFT JOIN MatchWinners W
			ON B.TeamCode = W.TeamCode
				AND B.MatchId = W.MatchId
				AND W.IsDeleted = 0
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
	WHERE B.MatchId = @MatchId
		AND B.Status = 'Final'		
END TRY
BEGIN CATCH    
    IF @@TRANCOUNT > 0
        ROLLBACK TRANSACTION;
END CATCH;

IF @@TRANCOUNT > 0
    COMMIT TRANSACTION;
";

    public ReverseBetsQuery(Guid matchId, Guid userId, Guid declareId)
    {
        CmdText = _query;

        Parameters.Add("MatchId", matchId);
        Parameters.Add("UserId", userId);
        Parameters.Add("DeclareId", declareId);
    }
}