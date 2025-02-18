using Remy.Gambit.Core.Data;

namespace Remy.Gambit.Data.Matches.DataQueries;

public class ProcessAgentCommissionsQuery : DataQuery
{
    private readonly string _query = @"
INSERT INTO Commissions(UserId, Amount, TransactionDate, TransactionType, TransactedBy, BetId, Commission, AgentCode)
SELECT 
	COALESCE(AG.UserId,COALESCE(MA.UserId, COALESCE(SMA.UserId, NULL))),
	CONVERT(MONEY, B.Amount * COALESCE(AG.Commission,COALESCE(MA.Commission, COALESCE(SMA.Commission, 0)))),
	GETUTCDATE(),
	'Betting',
	'System',
	B.Id,
	COALESCE(AG.Commission,COALESCE(MA.Commission, COALESCE(SMA.Commission, 0))),
	COALESCE(AG.AgentCode,COALESCE(MA.AgentCode, COALESCE(SMA.AgentCode, NULL)))
FROM Bets B
	JOIN Agency A
		ON B.UserId = A.UserId			
	LEFT JOIN Agency AG
		ON A.Upline = AG.UserId
	LEFT JOIN Agency MA
		ON (AG.Upline = MA.UserId OR A.Upline = MA.UserId)
			AND MA.AgentCode = 'MA'
	LEFT JOIN Agency SMA
		ON (MA.Upline = SMA.UserId OR A.Upline = SMA.UserId)
			AND SMA.AgentCode = 'SMA'
	JOIN AgentTypes AT
		ON AG.AgentCode = AT.Code
	LEFT JOIN Commissions C
		ON B.Id = C.BetId
			AND B.UserId = COALESCE(AG.UserId,COALESCE(MA.UserId, COALESCE(SMA.UserId, '')))
WHERE B.MatchId = @MatchId
	AND B.Status = 'Open'
	AND C.BetId IS NULL
";

    public ProcessAgentCommissionsQuery(Guid matchId)
    {
			CmdText = _query;

			Parameters.Add("MatchId", matchId);
    }
}
