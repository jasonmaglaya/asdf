using Remy.Gambit.Core.Data;

namespace Remy.Gambit.Data.Roles.DataQueries;

public class GetRolesQuery : DataQuery
{
    private readonly string _query = @"
SELECT *
FROM Roles
WHERE IsActive = 1
    AND Code <> 'sa' 
";

    public GetRolesQuery()
    {
        CmdText = _query;
    }
}
