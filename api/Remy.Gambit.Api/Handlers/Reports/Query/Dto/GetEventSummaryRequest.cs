using Remy.Gambit.Core.Cqs;
using System.Text.Json.Serialization;

namespace Remy.Gambit.Api.Handlers.Reports.Query.Dto;

public class GetEventSummaryRequest : IQuery
{
    [JsonIgnore]
    public Guid EventId { get; set; }
}
