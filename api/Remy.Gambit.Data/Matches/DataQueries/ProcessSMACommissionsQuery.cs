using Remy.Gambit.Core.Data;

namespace Remy.Gambit.Data.Matches.DataQueries;

public class ProcessSMACommissionsQuery : DataQuery
{
    private readonly string _query = @"
BEGIN TRANSACTION;

BEGIN TRY
	INSERT INTO Commissions(UserId, Amount, TransactionDate, TransactionType, TransactedBy, BetId, Commission, AgentCode)
	SELECT
		SMA.UserId,
		CONVERT(MONEY, B.Amount * (SMA.Commission - MA.Commission)),
		GETUTCDATE(),
		'Betting',
		'System',
		B.Id,
		SMA.Commission,
		SMA.AgentCode
	FROM Bets B
		JOIN Agency A
			ON B.UserId = A.UserId
		LEFT JOIN Agency AG
			ON A.Upline = AG.UserId
		JOIN Agency MA
			ON (AG.Upline = MA.UserId OR A.Upline = MA.UserId)
				AND MA.AgentCode = 'MA'
		JOIN Agency SMA
			ON (MA.Upline = SMA.UserId OR A.Upline = SMA.UserId)
				AND SMA.AgentCode = 'SMA'
		JOIN AgentTypes AT
			ON AG.AgentCode = AT.Code
		LEFT JOIN Commissions C
			ON B.Id = C.BetId
				AND B.UserId = SMA.UserId
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

    public ProcessSMACommissionsQuery(Guid matchId)
    {
			CmdText = _query;

			Parameters.Add("MatchId", matchId);
    }
}
