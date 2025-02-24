using Remy.Gambit.Core.Data;

namespace Remy.Gambit.Data.Events.DataQueries;

public class GetEventsQuery : DataQuery
{
    private static bool _includeNew;
    private readonly string _query = @"
DECLARE @pn INT
SET @pn = COALESCE(@PageNumber, 1)

DECLARE @ps INT
SET @ps = COALESCE(@PageSize, 20)

SELECT * 
FROM Events 
WHERE
    Status = COALESCE(@Status, Status)"
+ (!_includeNew ? " AND Status <> 'New'" : "") +
@" ORDER BY
    EventDate
OFFSET (@pn-1)*@ps ROWS
FETCH NEXT @ps ROWS ONLY

SELECT COUNT(*) Count
FROM Events 
WHERE
    Status = COALESCE(@Status, Status) "
+ (!_includeNew ? "AND Status <> 'New'" : "")
;

    public GetEventsQuery(string? status, int? pageNumber, int? pageSize, bool includeNew)
    {
        _includeNew = includeNew;

        CmdText = _query;

        Parameters.Add("Status", status);
        Parameters.Add("PageNumber", pageNumber);
        Parameters.Add("PageSize", pageSize);            
    }
}
