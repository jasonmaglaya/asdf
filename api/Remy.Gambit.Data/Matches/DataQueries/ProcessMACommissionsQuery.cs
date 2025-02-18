using Remy.Gambit.Core.Data;

namespace Remy.Gambit.Data.Matches.DataQueries;

public class ProcessMACommissionsQuery : DataQuery
{
    private readonly string _query = @"
BEGIN TRANSACTION;

BEGIN TRY
	INSERT INTO Commissions(UserId, Amount, TransactionDate, TransactionType, TransactedBy, BetId, Commission, AgentCode)
	SELECT 
		MA.UserId,
		CONVERT(MONEY, B.Amount * (MA.Commission - AG.Commission)), 
		GETUTCDATE(),
		'Betting',
		'System',
		B.Id,
		MA.Commission,
		MA.AgentCode
	FROM Bets B
		JOIN Agency A
			ON B.UserId = A.UserId
		JOIN Agency AG
			ON A.Upline = AG.UserId
		JOIN Agency MA
			ON AG.Upline = MA.UserId
				AND MA.AgentCode = 'MA'
		JOIN AgentTypes AT
			ON AG.AgentCode = AT.Code
		LEFT JOIN Commissions C
			ON B.Id = C.BetId
				AND B.UserId = MA.UserId
	WHERE B.MatchId = @MatchId
		AND B.Status = 'Open'
		AND C.BetId IS NULL
END TRY
BEGIN CATCH    
    IF @@TRANCOUNT > 0
        ROLLBACK TRANSACTION;
END CATCH;

IF @@TRANCOUNT > 0
    COMMIT TRANSACTION;
";

    public ProcessMACommissionsQuery(Guid matchId)
    {
			CmdText = _query;

			Parameters.Add("MatchId", matchId);
    }
}
