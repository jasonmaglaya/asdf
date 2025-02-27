using Remy.Gambit.Core.Data;

namespace Remy.Gambit.Data.Users.DataQueries;

public class GetLastRoundIdQuery : DataQuery
{
    private readonly string _query = @"
SELECT LastRoundId
FROM Users
WHERE Id = @UserId
";

    public GetLastRoundIdQuery(Guid userId)
    {
        CmdText = _query;

        Parameters.Add("UserId", userId);
    }
}
