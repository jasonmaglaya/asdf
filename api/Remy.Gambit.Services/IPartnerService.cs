namespace Remy.Gambit.Services;

public interface IPartnerService
{
    Task<decimal> GetBalanceAsync(string username, string userToken, CancellationToken cancelationToken = default);
}
