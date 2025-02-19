using Remy.Gambit.Core.Cqs;
using System.Text.Json.Serialization;

namespace Remy.Gambit.Api.Handlers.Users.Query.Dto;

public class GetUserBalanceRequest : IQuery
{
    [JsonIgnore]
    public Guid UserId { get; set; }

    public string? UserToken { get; set; }

    public string? UserName { get; set; }    
}
