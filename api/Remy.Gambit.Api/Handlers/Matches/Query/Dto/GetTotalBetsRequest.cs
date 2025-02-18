using Remy.Gambit.Core.Cqs;
using System.Text.Json.Serialization;

namespace Remy.Gambit.Api.Handlers.Matches.Query.Dto
{
    public class GetTotalBetsRequest : IQuery
    {
        [JsonIgnore]
        public Guid MatchId { get; set; }
    }
}
