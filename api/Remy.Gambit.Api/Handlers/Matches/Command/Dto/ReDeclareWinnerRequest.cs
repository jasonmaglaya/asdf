using Remy.Gambit.Core.Cqs;
using System.Text.Json.Serialization;

namespace Remy.Gambit.Api.Handlers.Matches.Command.Dto
{
    public class ReDeclareWinnerRequest : ICommand
    {
        [JsonIgnore]
        public Guid MatchId { get; set; }

        [JsonIgnore]
        public Guid UserId { get; set; }

        public required IEnumerable<string> TeamCodes { get; set; }
    }
}
