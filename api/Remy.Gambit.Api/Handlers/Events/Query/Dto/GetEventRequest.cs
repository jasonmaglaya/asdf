using Remy.Gambit.Core.Cqs;
using System.Text.Json.Serialization;

namespace Remy.Gambit.Api.Handlers.Events.Query.Dto
{
    public class GetEventRequest : IQuery
    {
        [JsonIgnore]
        public required Guid Id{ get; set; }
    }
}
