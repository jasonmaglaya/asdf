using Remy.Gambit.Core.Cqs;
using System.Text.Json.Serialization;

namespace Remy.Gambit.Api.Handlers.Credits.Request.Dto;

public class GetCreditHistoryRequest : IQuery 
{
    [JsonIgnore]
    public Guid UserId { get; set; }

    public int PageNumber { get; set; } = 1;

    public int PageSize { get; set; } = 20;
}
