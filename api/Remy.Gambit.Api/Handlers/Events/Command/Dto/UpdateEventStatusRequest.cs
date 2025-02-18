using Remy.Gambit.Core.Cqs;
using System.Text.Json.Serialization;

namespace Remy.Gambit.Api.Handlers.Events.Command.Dto;

public class UpdateEventStatusRequest : ICommand
{
    [JsonIgnore]
    public Guid EventId { get; set; }

    public required string Status { get; set; }
}
