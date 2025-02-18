using System.Collections.Concurrent;

namespace Remy.Gambit.Core.Concurrency;

public class SemaphoreUserLockService : IUserLockService
{
    private readonly ConcurrentDictionary<string, SemaphoreSlim> _userLocks = new();
    private readonly TimeSpan _lockTimeout = TimeSpan.FromSeconds(10);

    public async Task<bool> AcquireLockAsync(string userId)
    {    
        var semaphore = _userLocks.GetOrAdd(userId, _ => new SemaphoreSlim(1, 1));

        try
        {
            return await semaphore.WaitAsync(_lockTimeout);            
        }
        catch (OperationCanceledException)
        {
            return false;
        }
    }

    public Task ReleaseLockAsync(string userId)
    {
        if (_userLocks.TryGetValue(userId, out var semaphore))
        {
            semaphore.Release();
            _userLocks.TryRemove(userId, out _);
        }

        return Task.CompletedTask;
    }
}