using Remy.Gambit.Core.Cqs;
using System.Text.Json.Serialization;

namespace Remy.Gambit.Api.Handlers.Events.Query.Dto;

public class GetEventWinnersRequest : IQuery
{
    [JsonIgnore]
    public Guid EventId { get; set; }
}