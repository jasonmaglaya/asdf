using Remy.Gambit.Core.Cqs;
using System.Text.Json.Serialization;

namespace Remy.Gambit.Api.Handlers.Matches.Query.Dto
{
    public class GetMatchRequest : IQuery
    {
        [JsonIgnore]
        public Guid MatchId { get; set; }        
    }
}
