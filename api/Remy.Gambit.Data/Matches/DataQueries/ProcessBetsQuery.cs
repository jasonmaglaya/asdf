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
	UPDATE Bets SET
		OddsMultiplier = OM.OddsMultiplier
	FROM Bets B
	JOIN (
		SELECT
			T.Code TeamCode,			
			@TotalBetsAfterComm / COALESCE(SUM(B.Amount),0) OddsMultiplier
		FROM Matches M
			JOIN Teams T
				ON M.EventId = T.EventId
			JOIN Bets B
				ON T.EventId = M.EventId
				AND T.Code = B.TeamCode
				AND B.MatchId = M.Id
		WHERE M.Id = @MatchId
		GROUP BY T.Code
		UNION
		SELECT 'D', @DrawMultiplier
	) OM
		ON B.TeamCode = OM.TeamCode
	WHERE MatchId = @MatchId

	DECLARE @GroupTransactionId UNIQUEIDENTIFIER = NEWID()

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
			CASE
				WHEN (
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
				) > 0 THEN 'WINNINGS' ELSE 'LOSSES'			
			END Notes,
			GETUTCDATE() TransactionDate,
			'Betting' TransactionType,
			'System' TransactedBy,
			B.Id BetId,
			@GroupTransactionId GroupTransactionId,
			@DeclareId DeclareId
		FROM Bets B
			LEFT JOIN MatchWinners W
				ON B.MatchId = W.MatchId
					AND W.IsDeleted = 0			
			LEFT JOIN Credits T
				ON B.Id = T.BetId
		WHERE B.MatchId = @MatchId
			AND B.Status = 'Open'
			AND T.Id IS NULL
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

    public ProcessBetsQuery(Guid matchId, Guid declareId)
    {
        CmdText = _query;

			Parameters.Add("MatchId", matchId);
			Parameters.Add("DeclareId", declareId);
    }
}
