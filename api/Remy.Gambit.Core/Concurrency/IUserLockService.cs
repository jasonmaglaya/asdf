namespace Remy.Gambit.Core.Concurrency;

public interface IUserLockService
{
    Task<bool> AcquireLockAsync(Guid userId);
    Task ReleaseLockAsync(Guid userId);
}
