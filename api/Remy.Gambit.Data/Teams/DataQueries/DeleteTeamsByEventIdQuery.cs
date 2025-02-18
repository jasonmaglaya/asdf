using Remy.Gambit.Core.Data;

namespace Remy.Gambit.Data.Teams.DataQueries;

public class DeleteTeamsByEventId : DataQuery
{
    private readonly string _query = @"
DELETE Teams
WHERE EventId = @EventId
";

    public DeleteTeamsByEventId(Guid eventId)
    {
        CmdText = _query;

        Parameters.Add("@EventId", eventId);        
    }
}
