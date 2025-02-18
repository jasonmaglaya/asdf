using Remy.Gambit.Api.Dto;
using Remy.Gambit.Core.Cqs;
using Remy.Gambit.Core.Generics;

namespace Remy.Gambit.Api.Handlers.Users.Query.Dto
{
    public class GetAllUsersResult : QueryResult<PaginatedList<User>>
    {
    }
}
