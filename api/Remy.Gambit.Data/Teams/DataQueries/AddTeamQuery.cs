using Remy.Gambit.Core.Data;
using Remy.Gambit.Models;

namespace Remy.Gambit.Data.Teams.DataQueries;

public class AddTeamQuery : DataQuery
{
    private readonly string _query = @"
INSERT INTO Teams
(
    EventId,
    Code,
    Name,
    Sequence,
    Color
)
VALUES
(
    @EventId,
    @Code,
    @Name,
    @Sequence,
    @Color
)
";
    public AddTeamQuery(Guid eventId, Team team)
    {
        CmdText = _query;

        Parameters.Add("@EventId", eventId);
        Parameters.Add("@Name", team.Name);
        Parameters.Add("@Code", team.Code);        
        Parameters.Add("@Sequence", team.Sequence);
        Parameters.Add("@Color", team.Color);
    }
}
