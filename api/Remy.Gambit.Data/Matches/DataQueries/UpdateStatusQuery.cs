using Remy.Gambit.Core.Data;

namespace Remy.Gambit.Data.Matches.DataQueries;

public class UpdateStatusQuery : DataQuery
{
    private readonly string _query = @"
UPDATE Matches SET
    Status = @Status
WHERE Id = @Id
";
    public UpdateStatusQuery(Guid id, string status)
    {
        CmdText = _query;

        Parameters.Add("Id", id);
        Parameters.Add("Status", status);
    }
}
