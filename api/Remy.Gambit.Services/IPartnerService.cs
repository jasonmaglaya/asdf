using Remy.Gambit.Models;
using Remy.Gambit.Services.Dto;

namespace Remy.Gambit.Services;

public interface IPartnerService
{
    Task<Balance> GetBalanceAsync(GetBalanceRequest request, CancellationToken cancellationToken = default);

    Task<CashInResult> CashInAsync(CashInRequest request, CancellationToken cancellationToken = default);

    Task<CashOutResult> CashOutAsync(CashOutRequest request, CancellationToken cancellationToken = default);
}
