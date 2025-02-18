using Remy.Gambit.Core.Data;

namespace Remy.Gambit.Data.Users.DataQueries;

public class GetAllUsersQuery : DataQuery
{
    private readonly string _query = @"
DECLARE @pn INT
SET @pn = COALESCE(@PageNumber, 1)

DECLARE @ps INT
SET @ps = COALESCE(@PageSize, 20)

SELECT U.*, dbo.fx_GetUserCredits(U.Id) Credits
FROM Users U
    JOIN Roles R
        ON U.Role = r.Code
WHERE IsDeleted = 0 
    AND R.IsAgent = COALESCE(@IsAgent,R.IsAgent)
    AND Role <> 'sa'
ORDER BY
    Username
OFFSET (@pn-1)*@ps ROWS
FETCH NEXT @ps ROWS ONLY

SELECT COUNT(*)
FROM Users U
    JOIN Roles R
        ON U.Role = r.Code
WHERE IsDeleted = 0 
    AND R.IsAgent = COALESCE(@IsAgent,R.IsAgent)
    AND Role <> 'sa'
";

    public GetAllUsersQuery(bool? isAgent, int pageNumber, int pageSize)
    {
        CmdText = _query;

        Parameters.Add("IsAgent", isAgent);
        Parameters.Add("PageNumber", pageNumber);
        Parameters.Add("PageSize", pageSize);
    }
}
