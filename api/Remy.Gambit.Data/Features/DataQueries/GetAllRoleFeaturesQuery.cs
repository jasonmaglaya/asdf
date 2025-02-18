using Remy.Gambit.Core.Data;

namespace Remy.Gambit.Data.Features.DataQueries;

public class GetAllRoleFeaturesQuery : DataQuery
{
    private readonly string _query = @"
SELECT *
FROM RoleFeatures
";

    public GetAllRoleFeaturesQuery()
    {
        CmdText = _query;
    }
}
