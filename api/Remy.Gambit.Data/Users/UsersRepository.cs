using Remy.Gambit.Core.Generics;
using Remy.Gambit.Data.Roles.DataQueries;
using Remy.Gambit.Data.Users.DataQueries;
using Remy.Gambit.Models;

namespace Remy.Gambit.Data.Users;

public class UsersRepository : IUsersRepository
{   
    private readonly IGambitDbClient _gambitDbClient;

    public UsersRepository(IGambitDbClient gambitDbClient)
    {
        _gambitDbClient = gambitDbClient;    
    }

    public async Task<bool> CheckUsernameAvailabilityAsync(string username, CancellationToken token)
    {
        var query = new CheckUsernameAvailabilityQuery(username);

        return await _gambitDbClient.ExecuteScalarAsync<int>(query, token) == 0;
    }

    public async Task<User> GetUserByIdAsync(Guid userId, CancellationToken token)
    {
        var query = new GetUserByIdQuery(userId);

        var user = await _gambitDbClient.GetFirstOrDefaultAsync<User>(query, token);

        if (user?.Role is not null)
        {
            var roleQuery = new GetRoleByCodeQuery(user.Role);
            user.UserRole = await _gambitDbClient.GetFirstOrDefaultAsync<Role>(roleQuery, token);
        }

        return user!;
    }

    public async Task<User> GetUserByUsernameAsync(string username, CancellationToken token)
    {
        var query = new GetUserByUsernameQuery(username);

        return await _gambitDbClient.GetFirstOrDefaultAsync<User>(query, token);
    }

    public async Task<User?> GetUserByReferralCodeAsync(string referralCode, CancellationToken token)
    {
        var query = new GetUserByReferralCodeQuery(referralCode);

        var user = await _gambitDbClient.GetFirstOrDefaultAsync<User>(query, token);

        if(user?.Role is not null)
        {
            var roleQuery = new GetRoleByCodeQuery(user.Role);
            user.UserRole = await _gambitDbClient.GetFirstOrDefaultAsync<Role>(roleQuery, token);
        }

        return user;
    }

    public async Task<User> GetUserByRefreshTokenAsync(string refeshToken, CancellationToken token)
    {
        var query = new GetUserByRefreshTokenQuery(refeshToken);

        return await _gambitDbClient.GetFirstOrDefaultAsync<User>(query, token);
    }

    public async Task<bool> SignUpAsync(string username, string? password, string? contactNumber, string role, Guid? upline, string? agentCode,  bool isActive = false, CancellationToken token = default)
    {
        var query = new SignUpQuery(username, password, contactNumber, role, upline, agentCode, isActive);

        return await _gambitDbClient.ExecuteAsync(query, token) > 0;
    }

    public async Task<bool> UpdateRefreshTokenAsync(Guid userId, string? refreshToken, DateTime? expiry, CancellationToken token)
    {
        var query = new UpdateRefreshTokenQuery(userId, refreshToken, expiry);

        return await _gambitDbClient.ExecuteAsync(query, token) > 0;
    }

    public async Task<IEnumerable<User>> GetDownLinesAsync(Guid agentUserId, CancellationToken token)
    {
        var query = new GetDownLinesQuery(agentUserId);

        return await _gambitDbClient.GetCollectionAsync<User>(query, token);
    }

    public async Task<bool> UpdateStatusAsync(Guid username, bool isActive, CancellationToken token)
    {
        var query = new UpdateStatusQuery(username, isActive);

        return await _gambitDbClient.ExecuteAsync(query, token) > 0;
    }

    public async Task<bool> UpdateBettingStatusAsync(Guid username, bool isBettingLocked, CancellationToken token)
    {
        var query = new UpdateBettingStatusQuery(username, isBettingLocked);

        return await _gambitDbClient.ExecuteAsync(query, token) > 0;
    }

    public async Task<bool> UpdateRoleAsync(Guid username, string roleCode, CancellationToken token)
    {
        var query = new UpdateRoleQuery(username, roleCode);

        return await _gambitDbClient.ExecuteAsync(query, token) > 0;
    }

    public async Task<bool> UpdateAgencyAsync(Guid username, string agentCode, double commission, string role, CancellationToken token)
    {
        var query = new UpdateAgencyQuery(username, agentCode, commission, role);

        return await _gambitDbClient.ExecuteAsync(query, token) > 0;
    }

    public async Task<PaginatedList<User>> GetAllUsersAsync(bool? isAgent, int pageNumber, int pageSize, CancellationToken token)
    {
        var query = new GetAllUsersQuery(isAgent, pageNumber, pageSize);

        var (users, total) = await _gambitDbClient.GetMultipleAsync<User, int>(query, token);

        return new PaginatedList<User> { List = users, PageSize = pageSize, TotalItems = total.FirstOrDefault() };
    }

    public async Task<bool> TransferCreditsAsync(Guid? from, Guid to, decimal amount, Guid transactedBy, string? notes, CancellationToken token)
    {
        var query = new TransferCreditsQuery(from, to, amount, transactedBy, notes);

        return await _gambitDbClient.ExecuteAsync(query, token) > 0;
    }

    public async Task<PaginatedList<User>> SearchUserAsync(string keyword, bool? isAgent, Guid? requestor, int pageNumber, int pageSize, CancellationToken token)
    {
        var query = new SearchUserQuery(keyword, isAgent, requestor, pageNumber, pageSize);

        var (users, total) = await _gambitDbClient.GetMultipleAsync<User, int>(query, token);

        return new PaginatedList<User> { List = users, PageSize = pageSize, TotalItems = total.FirstOrDefault() };
    }

    public async Task<bool> UpdateLastRoundIdAsync(Guid userId, string lastRoundId, CancellationToken token)
    {
        var query = new UpdateLastRoundIdQuery(userId, lastRoundId);

        return await _gambitDbClient.ExecuteAsync(query, token) > 0;
    }

    public async Task<string> GetLastRoundIdAsync(Guid userId, CancellationToken token)
    {
        var query = new GetLastRoundIdQuery(userId);
        return await _gambitDbClient.ExecuteScalarAsync<string>(query, token);
    }

    public async Task<bool> ChangePasswordAsync(Guid userId, string newPassword, CancellationToken token)
    {
        var query = new ChangePasswordQuery(userId, newPassword);
        return await _gambitDbClient.ExecuteAsync(query, token) > 0;
    }
}