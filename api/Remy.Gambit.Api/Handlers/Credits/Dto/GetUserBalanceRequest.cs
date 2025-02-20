using Remy.Gambit.Core.Cqs;
using System.Text.Json.Serialization;

namespace Remy.Gambit.Api.Handlers.Credits.Dto;

public class GetUserBalanceRequest : IQuery
{
    [JsonIgnore]
    public Guid UserId { get; set; }

    public string? PartnerToken { get; set; } 
}
