using Remy.Gambit.Core.Data;
using Remy.Gambit.Models;

namespace Remy.Gambit.Data.Events.DataQueries;

public class AddEventQuery : DataQuery
{
    private readonly string _query = @"
DECLARE @Id UNIQUEIDENTIFIER = NEWID()

INSERT INTO Events
(
    Id,
    Title,
    SubTitle,
    EventDate,
    Status,
    Video,
    MinimumBet,
    MaximumBet,
    AllowDraw,
    DrawMultiplier,
    MinDrawBet,
    MaxDrawBet,
    MaxWinners,
    Commission,
    AllowAgentBetting,
    AllowAdminBetting,
    Padding
)
VALUES
(
    @Id,
    @Title,
    @SubTitle,
    @EventDate,
    'New',
    @Video,
    @MinimumBet,
    @MaximumBet,
    @AllowDraw,
    @DrawMultiplier,
    @MinDrawBet,
    @MaxDrawBet,
    @MaxWinners,
    @Commission,
    @AllowAgentBetting,
    @AllowAdminBetting,
    @Padding
)

SELECT @Id
";

    public AddEventQuery(Event @event)
    {
        CmdText = _query;

        Parameters.Add("@Title", @event.Title);
        Parameters.Add("@SubTitle", @event.SubTitle);
        Parameters.Add("@EventDate", @event.EventDate);
        Parameters.Add("@Video", @event.Video);
        Parameters.Add("@MinimumBet", @event.MinimumBet);
        Parameters.Add("@MaximumBet", @event.MaximumBet);
        Parameters.Add("@AllowDraw", @event.AllowDraw);
        Parameters.Add("@DrawMultiplier", @event.DrawMultiplier);
        Parameters.Add("@MinDrawBet", @event.MinDrawBet);
        Parameters.Add("@MaxDrawBet", @event.MaxDrawBet);
        Parameters.Add("@MaxWinners", @event.MaxWinners);
        Parameters.Add("@Commission", @event.Commission);
        Parameters.Add("@AllowAgentBetting", @event.AllowAgentBetting);
        Parameters.Add("@AllowAdminBetting", @event.AllowAdminBetting);
        Parameters.Add("@Padding", @event.Padding);
    }
}
