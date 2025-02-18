using Remy.Gambit.Core.Cqs;
using System.Text.Json.Serialization;

namespace Remy.Gambit.Api.Handlers.Matches.Command.Dto
{
    public class DeclareWinnerRequest : ICommand
    {
        [JsonIgnore]
        public Guid MatchId { get; set; }

        public required IEnumerable<string> TeamCodes { get; set; }
    }
}
