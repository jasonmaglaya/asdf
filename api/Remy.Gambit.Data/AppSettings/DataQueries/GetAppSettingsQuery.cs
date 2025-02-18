using Remy.Gambit.Core.Data;

namespace Remy.Gambit.Data.Features.DataQueries;

public class GetAppSettingsQuery : DataQuery
{
    private readonly string _query = @"
SELECT *
FROM AppSettings
";

    public GetAppSettingsQuery()
    {
        CmdText = _query;
    }
}
