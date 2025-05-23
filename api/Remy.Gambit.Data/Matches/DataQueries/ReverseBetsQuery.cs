﻿using Remy.Gambit.Core.Data;

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

BEGIN TRANSACTION;

BEGIN TRY
	
	DECLARE @GroupTransactionId UNIQUEIDENTIFIER = NEWID()

	INSERT INTO Credits (UserId, Amount, TransactionDate, TransactionType, TransactedBy, BetId, Notes, GroupTransactionId, DeclareId)	
	SELECT C.UserId, C.Amount * -1 Amount, GETUTCDATE(), 'Betting-Rollback', @UserId, C.BetId, 'REVERSAL (RE-DECLARED)', @GroupTransactionId, @DeclareId
	FROM Credits C
		JOIN Bets B
			ON C.BetId = B.Id	
	WHERE B.MatchId = @MatchId

	INSERT INTO Credits (UserId, Amount, Notes, TransactionDate, TransactionType, TransactedBy, BetId, GroupTransactionId, DeclareId)
	SELECT * FROM (		
		SELECT
			B.UserId, 
			CASE 
				WHEN B.TeamCode = W.TeamCode
					THEN
						CASE WHEN W.TeamCode = 'D'
							THEN (B.Amount * (@DrawMultiplier - 1)) * (1 - @Commission)
						ELSE
							B.Amount * (B.OddsMultiplier - 1)
						END
				ELSE
					CASE WHEN W.TeamCode = 'D' THEN 0 ELSE B.Amount * -1 END
			END Amount,
			CASE WHEN (
				CASE 
					WHEN B.TeamCode = W.TeamCode
						THEN
							CASE WHEN W.TeamCode = 'D'
								THEN (B.Amount * (@DrawMultiplier - 1)) * (1 - @Commission)
							ELSE
								B.Amount * (B.OddsMultiplier - 1)
							END
					ELSE
						CASE WHEN W.TeamCode = 'D' THEN 0 ELSE B.Amount * -1 END
				END
			) > 0 THEN 'WINNINGS' ELSE 'LOSSES' END Notes,
			GETUTCDATE() TransactionDate, 
			'Betting' TransactionType, 
			@UserId TransactedBy, 
			B.Id BetId,				
			@GroupTransactionId GroupTransactionId, 
			@DeclareId DeclareId
		FROM Bets B
			LEFT JOIN MatchWinners W
				ON B.MatchId = W.MatchId
					AND W.IsDeleted = 0			
		WHERE B.MatchId = @MatchId
			AND B.Status = 'Final'
	) X
	WHERE X.Amount <> 0
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