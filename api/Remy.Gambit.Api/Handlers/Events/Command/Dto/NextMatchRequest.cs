using Remy.Gambit.Core.Cqs;
using System.Text.Json.Serialization;

namespace Remy.Gambit.Api.Handlers.Events.Command.Dto
{
    public class NextMatchRequest : ICommand
    {
        [JsonIgnore]
        public Guid EventId { get; set; }
    }
}
