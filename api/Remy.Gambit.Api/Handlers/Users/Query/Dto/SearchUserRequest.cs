using Remy.Gambit.Core.Cqs;
using System.Text.Json.Serialization;

namespace Remy.Gambit.Api.Handlers.Users.Query.Dto;

public class SearchUserRequest : IQuery
{
    [JsonIgnore]
    public Guid Requestor { get; set; }

    public required string Keyword { get; set; }

    public bool? IsAgent { get; set; }

    public int PageNumber { get; set; }

    public int PageSize { get; set; }
}
