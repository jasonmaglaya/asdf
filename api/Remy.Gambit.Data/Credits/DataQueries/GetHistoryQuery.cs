using Remy.Gambit.Core.Data;

namespace Remy.Gambit.Data.Credits.DataQueries;

public class GetHistoryQuery : DataQuery
{
    private readonly string _query = @"
DECLARE @pn INT
SET @pn = COALESCE(@PageNumber, 1)

DECLARE @ps INT
SET @ps = COALESCE(@PageSize, 20)

SELECT *
FROM Credits C
WHERE
	C.UserId = @UserId
	AND C.TransactionType = 'Loading'
ORDER BY TransactionDate DESC
OFFSET (@pn-1)*@ps ROWS
FETCH NEXT @ps ROWS ONLY

SELECT COUNT(*) Count
FROM Credits C
WHERE
	C.UserId = @UserId
	AND C.TransactionType = 'Loading'
    ";

    public GetHistoryQuery(Guid userId, int pageNumber, int pageSize)
    {
        CmdText = _query;

        Parameters.Add("@UserId", userId);
        Parameters.Add("@PageNumber", pageNumber);
        Parameters.Add("@PageSize", pageSize);
    }
}
