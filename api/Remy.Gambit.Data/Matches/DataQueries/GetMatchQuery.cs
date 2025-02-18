using Remy.Gambit.Core.Data;

namespace Remy.Gambit.Data.Matches.DataQueries;

public class GetMatchQuery : DataQuery
{
    private readonly string _query = @"
SELECT *
FROM Matches
WHERE Id = @Id
";
    public GetMatchQuery(Guid id)
    {
        CmdText = _query;

        Parameters.Add("Id", id);
    }
}
