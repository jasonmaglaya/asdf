namespace Remy.Gambit.Core.Concurrency;

public interface IUserLockService
{
    Task<bool> AcquireLockAsync(string userId);
    Task ReleaseLockAsync(string userId);
}
