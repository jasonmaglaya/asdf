using Remy.Gambit.Core.Cqs;
using System.Text.Json.Serialization;

namespace Remy.Gambit.Api.Handlers.Events.Query.Dto;

public class GetEventSummaryRequest : IQuery
{
    [JsonIgnore]
    public Guid EventId { get; set; }

    [JsonIgnore]
    public Guid UserId { get; set; }
}