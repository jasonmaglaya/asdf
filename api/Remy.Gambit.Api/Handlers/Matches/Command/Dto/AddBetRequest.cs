using Remy.Gambit.Core.Cqs;
using System.Text.Json.Serialization;

namespace Remy.Gambit.Api.Handlers.Matches.Command.Dto
{
    public class AddBetRequest : ICommand
    {
        [JsonIgnore]
        public Guid UserId { get; set; }

        [JsonIgnore]
        public Guid MatchId { get; set; }

        public string? TeamCode { get; set; }

        public decimal Amount { get; set; }
    }
}
