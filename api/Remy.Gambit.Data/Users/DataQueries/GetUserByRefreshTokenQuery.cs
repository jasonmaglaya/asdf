using Remy.Gambit.Core.Data;

namespace Remy.Gambit.Data.Users.DataQueries;

public class GetUserByRefreshTokenQuery : DataQuery 
{
    private readonly string _query = @"
SELECT * 
FROM Users 
WHERE
    RefreshToken = @RefreshToken
        AND RefreshTokenExpiry >= GETUTCDATE()
";

    public GetUserByRefreshTokenQuery(string refreshToken)
    {
        CmdText = _query;

        Parameters.Add("RefreshToken", refreshToken);
    }
}
