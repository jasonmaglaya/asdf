using Remy.Gambit.Models;

namespace Remy.Gambit.Services;

public interface IPartnerService
{
    Task<Balance> GetBalanceAsync(string partnerToken, CancellationToken cancelationToken = default);
}
