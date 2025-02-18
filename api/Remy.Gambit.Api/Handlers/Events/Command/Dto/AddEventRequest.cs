using Remy.Gambit.Api.Dto;
using Remy.Gambit.Core.Cqs;

namespace Remy.Gambit.Api.Handlers.Events.Command.Dto;

public class AddEventRequest : ICommand
{
    public required AddEventDto Event { get; set; }
    public required IEnumerable<Team> Teams { get; set; }
}
