using Remy.Gambit.Core.Data;

namespace Remy.Gambit.Data.Roles.DataQueries;

public class GetRoleByCodeQuery : DataQuery
{
    private readonly string _query = @"
SELECT *
FROM Roles
WHERE Code = @Code
";

    public GetRoleByCodeQuery(string code)
    {
        CmdText = _query;

        Parameters.Add("Code", code);
    }
}
