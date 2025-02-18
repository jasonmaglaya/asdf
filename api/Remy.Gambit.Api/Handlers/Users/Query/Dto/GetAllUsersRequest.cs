using Remy.Gambit.Core.Cqs;

namespace Remy.Gambit.Api.Handlers.Users.Query.Dto;

public class GetAllUsersRequest : IQuery
{
    public bool? IsAgent { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}
