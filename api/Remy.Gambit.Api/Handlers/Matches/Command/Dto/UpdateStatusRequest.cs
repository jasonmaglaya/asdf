using Remy.Gambit.Core.Cqs;
using System.Text.Json.Serialization;

namespace Remy.Gambit.Api.Handlers.Matches.Command.Dto
{
    public class UpdateStatusRequest : ICommand
    {
        [JsonIgnore]
        public Guid MatchId { get; set; }

        public Guid EventId { get; set; }

        public required string Status { get; set; }
    }
}
