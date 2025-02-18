using Remy.Gambit.Data.Roles.DataQueries;
using Remy.Gambit.Models;

namespace Remy.Gambit.Data.Roles;

public class RolesRepository : IRolesRepository
{
    private readonly IGambitDbClient _gambitDbClient;

    public RolesRepository(IGambitDbClient gambitDbClient)
    {
        _gambitDbClient = gambitDbClient;
    }

    public async Task<IEnumerable<Role>> GetRolesAsync(CancellationToken token)
    {
        var query = new GetRolesQuery();

        return await _gambitDbClient.GetCollectionAsync<Role>(query, token);
    }
}
