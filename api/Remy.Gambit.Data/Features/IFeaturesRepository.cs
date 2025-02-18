using Remy.Gambit.Models;

namespace Remy.Gambit.Data.Features;

public interface IFeaturesRepository
{
    Task<IEnumerable<string>> GetFeaturesByRoleAsync(string role, CancellationToken token);
}
