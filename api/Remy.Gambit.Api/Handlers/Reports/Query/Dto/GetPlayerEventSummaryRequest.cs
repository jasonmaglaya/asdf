using Remy.Gambit.Core.Cqs;
using System.Text.Json.Serialization;

namespace Remy.Gambit.Api.Handlers.Reports.Query.Dto;

public class GetPlayerEventSummaryRequest : IQuery
{
    [JsonIgnore]
    public Guid EventId { get; set; }

    [JsonIgnore]
    public Guid UserId { get; set; }
}