using Remy.Gambit.Api.Dto;
using Remy.Gambit.Core.Cqs;

namespace Remy.Gambit.Api.Handlers.Events.Command.Dto;

public class UpdateEventRequest : ICommand
{
    public required UpdateEventDto Event { get; set; }
    public required IEnumerable<Team> Teams { get; set; }
}
