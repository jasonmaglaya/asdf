namespace Remy.Gambit.Services;

public interface IPartnerService
{
    Task<string> GetBalanceAsync(string username, string userToken, CancellationToken cancelationToken = default);
}
