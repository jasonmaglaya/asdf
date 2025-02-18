using Remy.Gambit.Core.Cqs;

namespace Remy.Gambit.Api.Handlers.Events.Command.Dto;

public class AddEventResult : CommandResult
{
    public Guid EventId { get; set; }
}
