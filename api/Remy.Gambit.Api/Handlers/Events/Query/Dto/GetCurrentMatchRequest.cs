using Remy.Gambit.Core.Cqs;
using System.Text.Json.Serialization;

namespace Remy.Gambit.Api.Handlers.Events.Query.Dto
{
    public class GetCurrentMatchRequest : IQuery
    {
        [JsonIgnore]
        public required Guid EventId{ get; set; }
    }
}
