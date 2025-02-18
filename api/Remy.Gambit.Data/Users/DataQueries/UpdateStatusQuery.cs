using Remy.Gambit.Core.Data;

namespace Remy.Gambit.Data.Users.DataQueries;

public class UpdateStatusQuery : DataQuery
{
    private readonly string _query = @"
UPDATE Users SET
    IsActive = @IsActive
WHERE Id = @UserId
";

    public UpdateStatusQuery(Guid userId, bool isActive)
    {
        CmdText = _query;

        Parameters.Add("UserId", userId);
        Parameters.Add("IsActive", isActive);
    }
}
