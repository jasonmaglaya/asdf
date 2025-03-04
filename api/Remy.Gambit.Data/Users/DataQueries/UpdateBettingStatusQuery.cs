using Remy.Gambit.Core.Data;

namespace Remy.Gambit.Data.Users.DataQueries;

public class UpdateBettingStatusQuery : DataQuery
{
    private readonly string _query = @"
UPDATE Users SET
    IsBettingLocked = @IsBettingLocked
WHERE Id = @UserId
";

    public UpdateBettingStatusQuery(Guid userId, bool isBettingLocked)
    {
        CmdText = _query;

        Parameters.Add("UserId", userId);
        Parameters.Add("IsBettingLocked", isBettingLocked);
    }
}
