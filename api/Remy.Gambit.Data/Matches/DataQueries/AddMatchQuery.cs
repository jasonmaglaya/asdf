using Remy.Gambit.Core.Data;
using Remy.Gambit.Models;

namespace Remy.Gambit.Data.Matches.DataQueries;

public class AddMatchQuery : DataQuery
{
    private readonly string _query = $@"
DECLARE @Id UNIQUEIDENTIFIER = NEWID()

INSERT INTO Matches (Id, EventId, Number, Description, Status, Sequence)
VALUES (@Id, @EventId, @Number, @Description, 'New', @Sequence)

UPDATE Events SET
    CurrentMatch = @Id
WHERE Id = @EventId
";

    public AddMatchQuery(Guid eventId, Match match)
    {
        CmdText = _query;

        Parameters.Add("EventId", eventId);
        Parameters.Add("Number", match.Number);
        Parameters.Add("Description", match.Description);
        Parameters.Add("Sequence", match.Sequence);
    }
}
