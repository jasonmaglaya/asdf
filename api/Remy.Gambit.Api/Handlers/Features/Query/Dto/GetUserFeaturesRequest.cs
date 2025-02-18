using Remy.Gambit.Core.Cqs;
using System.Text.Json.Serialization;

namespace Remy.Gambit.Api.Handlers.Features.Query.Dto
{
    public class GetUserFeaturesRequest : IQuery
    {
        [JsonIgnore]
        public string? Role { get; set; }
    }
}
