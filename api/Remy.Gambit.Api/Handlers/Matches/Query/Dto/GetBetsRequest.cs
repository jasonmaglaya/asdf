using Remy.Gambit.Core.Cqs;
using System.Text.Json.Serialization;

namespace Remy.Gambit.Api.Handlers.Matches.Query.Dto
{
    public class GetBetsRequest : IQuery
    {
        [JsonIgnore]
        public Guid MatchId { get; set; }

        [JsonIgnore]
        public Guid UserId { get; set; }
    }
}
