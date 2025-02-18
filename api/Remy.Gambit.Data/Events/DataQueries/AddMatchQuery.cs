using Remy.Gambit.Core.Data;

namespace Remy.Gambit.Data.Events.DataQueries;

public class AddMatchQuery : DataQuery
{
    private readonly string _query = @"
DECLARE @EventStatus VARCHAR(20)
SELECT @EventStatus=Status FROM Events WHERE Id = @EventId

IF @EventStatus = 'Closed' RETURN

DECLARE @Id UNIQUEIDENTIFIER = NEWID()

IF @Number IS NULL
BEGIN
    SELECT @Number=COALESCE(MAX(Number),0) + 1
    FROM Matches
    WHERE EventId = @EventId
END

DECLARE @Sequence SMALLINT
SELECT @Sequence=COALESCE(MAX(Sequence),0) + 1
FROM Matches
WHERE EventId = @EventId

INSERT INTO Matches (Id, EventId, Number, Description, Status, Sequence)
VALUES (@Id, @EventId, @Number, @Description, 'New', @Sequence)

UPDATE Events SET
    CurrentMatch = @Id
WHERE Id = @EventId

SELECT @Id AS Id
";

    public AddMatchQuery(Guid eventId, int? number = null, string? description = null)
    {
        CmdText = _query;

        Parameters.Add("EventId", eventId);
        Parameters.Add("Number", number);
        Parameters.Add("Description", description);
    }
}
