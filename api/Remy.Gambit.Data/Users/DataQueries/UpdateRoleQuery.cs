using Remy.Gambit.Core.Data;

namespace Remy.Gambit.Data.Users.DataQueries;

public class UpdateRoleQuery : DataQuery
{
    private readonly string _query = @"
UPDATE Users SET
    Role = @RoleCode
WHERE Id = @UserId
";

    public UpdateRoleQuery(Guid userId, string roleCode)
    {
        CmdText = _query;

        Parameters.Add("UserId", userId);
        Parameters.Add("RoleCode", roleCode);
    }
}
