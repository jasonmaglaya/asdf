using Remy.Gambit.Core.Data;

namespace Remy.Gambit.Data.Matches.DataQueries;

public class AddBetQuery : DataQuery
{
    private readonly string _query = @"
BEGIN TRANSACTION;

BEGIN TRY
    DECLARE @Credits MONEY = dbo.fx_GetUserCredits(@UserId)

	IF @Credits < @Amount RETURN

	INSERT INTO Bets (UserId, MatchId, TeamCode, Amount, BetTimeStamp, Status, IpAddress)
	VALUES (@UserId, @MatchId, @TeamCode, @Amount, GETUTCDATE(), 'Open', @IpAddress)

	SELECT dbo.fx_GetUserCredits(@UserId) AS Credits
END TRY
BEGIN CATCH
    
    IF @@TRANCOUNT > 0
        ROLLBACK TRANSACTION;
END CATCH;

IF @@TRANCOUNT > 0
    COMMIT TRANSACTION;
";
    public AddBetQuery(Guid userId, Guid matchId, string teamCode, decimal amount, string ipAddress)
    {
        CmdText = _query;

        Parameters.Add("UserId", userId);
        Parameters.Add("MatchId", matchId);
        Parameters.Add("TeamCode", teamCode);
        Parameters.Add("Amount", amount);
        Parameters.Add("IpAddress", ipAddress);
    }
}
