using Remy.Gambit.Core.Data;

namespace Remy.Gambit.Data.Events.DataQueries;

public class GetEventQuery : DataQuery
{
    private readonly string _query = @"
SELECT * 
FROM Events 
WHERE
    Id = @Id

SELECT *
FROM Teams
WHERE EventId = @Id
";

    public GetEventQuery(Guid id)
    {
        CmdText = _query;

        Parameters.Add("Id", id);
    }
}
