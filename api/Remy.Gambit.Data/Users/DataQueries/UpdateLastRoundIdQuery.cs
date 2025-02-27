using Remy.Gambit.Core.Data;

namespace Remy.Gambit.Data.Users.DataQueries;

public class UpdateLastRoundIdQuery : DataQuery
{
    private readonly string _query = @"
UPDATE Users SET
    LastRoundId = @LastRoundId
WHERE Id = @UserId
";

    public UpdateLastRoundIdQuery(Guid userId, string lastRoundId)
    {
        CmdText = _query;

        Parameters.Add("UserId", userId);
        Parameters.Add("LastRoundId", lastRoundId);
    }
}
