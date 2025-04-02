using Remy.Gambit.Core.Data;

namespace Remy.Gambit.Data.Credits.DataQueries;

public class CanCashOutQuery : DataQuery
{
    private readonly string _query = @"
DECLARE @EventId UNIQUEIDENTIFIER
DECLARE @LastBet INT

SELECT TOP 1 @LastBet = M.Number, @EventId = E.Id
FROM Bets B
	JOIN Matches M
		ON B.MatchId = M.Id
	JOIN [Events] E
		ON M.EventId = E.Id
WHERE UserId = @UserId
	AND E.[Status] <> 'Final'
ORDER BY B.BetTimeStamp DESC

IF @EventId IS NULL
BEGIN
	SELECT 1
	RETURN
END

DECLARE @CurrentMatch INT
SELECT TOP 1 @CurrentMatch = Number
FROM Matches M		
	JOIN [Events] E
		ON M.EventId = E.Id
WHERE E.Id = @EventID
ORDER BY Number DESC

DECLARE @NumberOfMatches INT
SELECT @NumberOfMatches = [Value]
FROM AppSettings
WHERE SettingKey = 'MatchesToCashOut'

IF (@CurrentMatch - @LastBet) >= @NumberOfMatches
BEGIN
	SELECT 1
	RETURN
END

SELECT 0
";

    public CanCashOutQuery(Guid userId)
    {
        Parameters.Add("@UserId", userId);

		CmdText = _query;
    }
}
