using Remy.Gambit.Core.Concurrency;
using System.Collections.Concurrent;

namespace Remy.Gambit.Api.Concurrencies;

public class SemaphoreUserLockService : IUserLockService
{
    private readonly ConcurrentDictionary<string, SemaphoreSlim> _userLocks = new();
    private readonly TimeSpan _lockTimeout = TimeSpan.FromSeconds(10);

    public async Task<bool> AcquireLockAsync(Guid userId)
    {    
        var semaphore = _userLocks.GetOrAdd(userId.ToString(), _ => new SemaphoreSlim(1, 1));

        try
        {
            return await semaphore.WaitAsync(_lockTimeout);            
        }
        catch (OperationCanceledException)
        {
            return false;
        }
    }

    public Task ReleaseLockAsync(Guid userId)
    {
        if (_userLocks.TryGetValue(userId.ToString(), out var semaphore))
        {
            semaphore.Release();
            _userLocks.TryRemove(userId.ToString(), out _);
        }

        return Task.CompletedTask;
    }
}