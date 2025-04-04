using Remy.Gambit.Core.Data;

namespace Remy.Gambit.Data.Users.DataQueries;

public class ChangePasswordQuery : DataQuery
{
    private readonly string _query = @"
UPDATE Users SET
    Password = @Password
WHERE
    Id = @UserId
";

    public ChangePasswordQuery(Guid userId, string newPassword)
    {
        Parameters.Add("UserId", userId);
        Parameters.Add("Password", newPassword);

        CmdText = _query;
    }
}
