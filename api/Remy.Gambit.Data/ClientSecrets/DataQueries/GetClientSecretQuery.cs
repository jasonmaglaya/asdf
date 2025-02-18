using Remy.Gambit.Core.Data;

namespace Remy.Gambit.Data.ClientSecrets.DataQueries;

public class GetClientSecretQuery : DataQuery
{
    private readonly string _query = @"
SELECT *
FROM ClientSecrets
WHERE Id = @Id
";

    public GetClientSecretQuery(string id)
    {
        Parameters.Add("Id", id);

        CmdText = _query;
    }
}