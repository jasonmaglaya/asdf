using Remy.Gambit.Core.Generics;
using Remy.Gambit.Models;

namespace Remy.Gambit.Data.Users;

public interface IUsersRepository
{
    Task<User> GetUserByIdAsync(Guid userId, CancellationToken token);

    Task<User> GetUserByUsernameAsync(string username, CancellationToken token);

    Task<User?> GetUserByReferralCodeAsync(string referralCode, CancellationToken token);

    Task<bool> SignUpAsync(string username, string? password, string? contactNumber, string role, Guid? upline, string? agentCode, bool isActive = false, CancellationToken token = default);

    Task<bool> CheckUsernameAvailabilityAsync(string username, CancellationToken token);

    Task<User> GetUserByRefreshTokenAsync(string refreshToken, CancellationToken token);

    Task<bool> UpdateRefreshTokenAsync(Guid userId, string? refreshToken, DateTime? expiry, CancellationToken token);

    Task<IEnumerable<User>> GetDownLinesAsync(Guid agentUserId, CancellationToken token);

    Task<bool> UpdateStatusAsync(Guid userId, bool isActive, CancellationToken token);

    Task<bool> UpdateBettingStatusAsync(Guid userId, bool isBettingLocked, CancellationToken token);

    Task<bool> UpdateRoleAsync(Guid userId, string roleCode, CancellationToken token);

    Task<bool> UpdateAgencyAsync(Guid userId, string agentCode, double commission, string role, CancellationToken token);

    Task<PaginatedList<User>> GetAllUsersAsync(bool? isAgent, int pageNumber, int pageSize, CancellationToken token);

    Task<bool> TransferCreditsAsync(Guid? from, Guid to, decimal amount, Guid transactedBy, string? notes, CancellationToken token);
    
    Task<PaginatedList<User>> SearchUserAsync(string keyword, bool? isAgent, Guid? requestor, int pageNumber, int pageSize, CancellationToken token);

    Task<bool> UpdateLastRoundIdAsync(Guid userId, string lastRoundId, CancellationToken token);

    Task<string> GetLastRoundIdAsync(Guid userId, CancellationToken token);

    Task<bool> ChangePasswordAsync(Guid userId, string newPassword, CancellationToken token);
}
