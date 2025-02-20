using Remy.Gambit.Models;

namespace Remy.Gambit.Data.Credits;

public interface ICreditsRepository
{
    Task<bool> CashInAsync(Credit credit, CancellationToken cancellationToken);
    Task<bool> CashOutAsync(Credit credit, CancellationToken cancellationToken);
}
