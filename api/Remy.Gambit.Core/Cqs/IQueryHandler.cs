namespace Remy.Gambit.Core.Cqs;

public interface IQueryHandler<in T1, T2> where T1 : IQuery
                                          where T2 : IQueryResult
{
    ValueTask<T2> HandleAsync(T1 request, CancellationToken token = default);
}
