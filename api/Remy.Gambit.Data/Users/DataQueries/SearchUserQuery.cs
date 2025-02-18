using Remy.Gambit.Core.Data;

namespace Remy.Gambit.Data.Users.DataQueries;

public class SearchUserQuery : DataQuery
{
    private readonly string _query = @"
DECLARE @pn INT
SET @pn = COALESCE(@PageNumber, 1)

DECLARE @ps INT
SET @ps = COALESCE(@PageSize, 20)

DECLARE @DUMMY_ID UNIQUEIDENTIFIER = NEWID()

SELECT U.*, dbo.fx_GetUserCredits(U.Id) Credits
FROM Users U
    JOIN Roles R
        ON U.Role = R.Code
    LEFT JOIN Agency A
        ON U.Id = A.UserId
WHERE U.Username like '%' + @Keyword + '%'
    AND R.IsAgent = COALESCE(@IsAgent,R.IsAgent)
    AND COALESCE(A.Upline, @DUMMY_ID) = COALESCE(@Requestor, COALESCE(A.Upline, @DUMMY_ID))
    AND IsDeleted = 0    
    AND U.Role <> 'sa'
ORDER BY
    Username
OFFSET (@pn-1)*@ps ROWS
FETCH NEXT @ps ROWS ONLY

SELECT COUNT(*)
FROM Users U
    JOIN Roles R
        ON U.Role = R.Code
    LEFT JOIN Agency A
        ON U.Id = A.UserId
WHERE U.Username like '%' + @Keyword + '%'
    AND R.IsAgent = COALESCE(@IsAgent,R.IsAgent)
    AND COALESCE(A.Upline, @DUMMY_ID) = COALESCE(@Requestor, COALESCE(A.Upline, @DUMMY_ID))
    AND IsDeleted = 0
    AND U.Role <> 'sa'
";

    public SearchUserQuery(string keyword, bool? isAgent, Guid? requestor, int pageNumber, int pageSize)
    {
        CmdText = _query;

        Parameters.Add("Keyword", keyword);
        Parameters.Add("IsAgent", isAgent);
        Parameters.Add("Requestor", requestor);
        Parameters.Add("PageNumber", pageNumber);
        Parameters.Add("PageSize", pageSize);
    }
}
