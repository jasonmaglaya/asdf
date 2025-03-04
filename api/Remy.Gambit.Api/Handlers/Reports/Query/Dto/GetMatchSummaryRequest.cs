using Remy.Gambit.Core.Cqs;
using System.Text.Json.Serialization;

namespace Remy.Gambit.Api.Handlers.Reports.Query.Dto;

public class GetMatchSummaryRequest : IQuery
{
    [JsonIgnore]
    public Guid MatchId { get; set; }    
}