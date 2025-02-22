using Remy.Gambit.Core.Generics;
using Remy.Gambit.Models;

namespace Remy.Gambit.Data.Credits;

public interface ICreditsRepository
{
    Task<bool> CashInAsync(Credit credit, string notes, CancellationToken cancellationToken);
    Task<bool> CashOutAsync(Credit credit, string notes, CancellationToken cancellationToken);
    Task<PaginatedList<Credit>> GetHistoryAsync(Guid userId, int pageNumber, int pageSize, CancellationToken token);
}
