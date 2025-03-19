using Remy.Gambit.Core.Data;

namespace Remy.Gambit.Data.Events.DataQueries;

public class UpdateEventStatusQuery : DataQuery
{
    private readonly string _query = @"
UPDATE Events
SET Status = @Status
WHERE Id = @EventId
    AND Status != 'Final'
";

    public UpdateEventStatusQuery(Guid eventId, string status)
    {
        CmdText = _query;

        Parameters.Add("@EventId", eventId);
        Parameters.Add("@Status", status);
    }
}
