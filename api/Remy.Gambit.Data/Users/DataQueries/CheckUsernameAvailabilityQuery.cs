using Remy.Gambit.Core.Data;

namespace Remy.Gambit.Data.Users.DataQueries;

public class CheckUsernameAvailabilityQuery : DataQuery
{
    private readonly string _query = @"
SELECT COUNT(Username)
FROM Users 
WHERE Username = @Username
";

    public CheckUsernameAvailabilityQuery(string username)
    {
        CmdText = _query;

        Parameters.Add("Username", username);
    }
}
