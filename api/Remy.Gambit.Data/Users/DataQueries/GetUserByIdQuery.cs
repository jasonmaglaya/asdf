using Remy.Gambit.Core.Data;

namespace Remy.Gambit.Data.Users.DataQueries;

public class GetUserByIdQuery : DataQuery 
{
    private readonly string _query = @"
DECLARE @Credits MONEY = dbo.fx_GetUserCredits(@UserId)

SELECT U.*, A.Upline, A.Commission, A.AgentCode, @Credits as Credits
FROM Users U
    LEFT JOIN Agency A
        ON U.Id = A.UserId
WHERE
    U.Id = @UserId
";

    public GetUserByIdQuery(Guid userId)
    {
        CmdText = _query;

        Parameters.Add("UserId", userId);
    }
}
