using Remy.Gambit.Core.Data;
using Remy.Gambit.Models;

namespace Remy.Gambit.Data.Events.DataQueries;

public class UpdateEventQuery : DataQuery
{
    private readonly string _query = @"
UPDATE Events SET
    Title = @Title,
    SubTitle = @SubTitle,
    EventDate = @EventDate,
    Video = @Video,
    MinimumBet = @MinimumBet,
    MaximumBet = @MaximumBet,
    AllowDraw = @AllowDraw,
    DrawMultiplier = @DrawMultiplier,
    MinDrawBet = @MinDrawBet,
    MaxDrawBet = @MaxDrawBet,
    MaxWinners = @MaxWinners,
    Commission = @Commission,
    AllowAgentBetting = @AllowAgentBetting,
    AllowAdminBetting = @AllowAdminBetting,
    Padding = @Padding
WHERE Id = @Id
    AND Status != 'Closed'
";

    public UpdateEventQuery(Event @event)
    {
        CmdText = _query;

        Parameters.Add("@Id", @event.Id);
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
