using Remy.Gambit.Core.Cqs;
using System.Text.Json.Serialization;

namespace Remy.Gambit.Api.Handlers.Matches.Command.Dto
{
    public class CancelMatchRequest : ICommand
    {
        public Guid EventId { get; set; }

        [JsonIgnore]
        public Guid MatchId { get; set; }

        [JsonIgnore]
        public Guid CancelledBy { get; set; }

        [JsonIgnore]
        public string? IpAddress { get; set; }
    }
}
