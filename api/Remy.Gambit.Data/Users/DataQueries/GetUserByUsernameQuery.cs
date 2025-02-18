using Remy.Gambit.Core.Data;

namespace Remy.Gambit.Data.Users.DataQueries;

public class GetUserByUsernameQuery : DataQuery 
{
    private readonly string _query = @"
SELECT *
FROM Users
WHERE
    Username = @Username
";

    public GetUserByUsernameQuery(string username)
    {
        CmdText = _query;

        Parameters.Add("Username", username);
    }
}
