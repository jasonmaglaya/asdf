using Remy.Gambit.Core.Cqs;
using System.Text.Json.Serialization;

namespace Remy.Gambit.Api.Handlers.Users.Query.Dto
{
    public class GetDownLinesRequest : IQuery
    {
        [JsonIgnore]
        public Guid UserId { get; set; }
    }
}
