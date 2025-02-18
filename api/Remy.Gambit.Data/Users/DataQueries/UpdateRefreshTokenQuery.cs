using Remy.Gambit.Core.Data;

namespace Remy.Gambit.Data.Users.DataQueries;

public class UpdateRefreshTokenQuery : DataQuery
{
    private readonly string _query = @"
UPDATE Users SET
    RefreshToken = @RefreshToken,
    RefreshTokenExpiry = @RefreshTokenExpiry
WHERE
    Id = @UserId
";

    public UpdateRefreshTokenQuery(Guid userId, string? refreshToken, DateTime? expiry)
    {
        CmdText = _query;

        Parameters.Add("UserId", userId);
        Parameters.Add("RefreshToken", refreshToken);
        Parameters.Add("RefreshTokenExpiry", expiry);
    }
}
