using Remy.Gambit.Models;

namespace Remy.Gambit.Data.Roles;

public interface IRolesRepository
{
    Task<IEnumerable<Role>> GetRolesAsync(CancellationToken token);
}
