using Remy.Gambit.Api.Dto;
using Remy.Gambit.Core.Cqs;
using Remy.Gambit.Core.Generics;

namespace Remy.Gambit.Api.Handlers.Credits.Request.Dto;

public class GetCreditHistoryResult : QueryResult<PaginatedList<CreditHistoryItem>>
{

}
