using Dapper;

namespace Remy.Gambit.Core.Data;

public interface IDataClient
{
    ValueTask<T> GetFirstOrDefaultAsync<T>(IDataQuery query, CancellationToken token = default);

    ValueTask<IEnumerable<T>> GetCollectionAsync<T>(IDataQuery query, CancellationToken token = default);

    ValueTask<int> ExecuteAsync(IDataQuery command, CancellationToken token = default);

    ValueTask<T> ExecuteScalarAsync<T>(IDataQuery command, CancellationToken token = default);

    ValueTask<int> ExecuteProcedureAsync(IDataQuery command, CancellationToken token = default);

    ValueTask<T> ExecuteScalarProcedureAsync<T>(IDataQuery command, CancellationToken token = default);

    ValueTask<(IEnumerable<T1> FirstResult, IEnumerable<T2> SecondResult)> GetMultipleAsync<T1, T2>(
        IDataQuery query,
        CancellationToken token = default);

    ValueTask<(IEnumerable<T1> FirstResult, IEnumerable<T2> SecondResult, IEnumerable<T3> ThirdResult)> GetMultipleAsync<T1, T2, T3>(
        IDataQuery query,
        CancellationToken token = default);
    ValueTask<(IEnumerable<T1> FirstResult, IEnumerable<T2> SecondResult, IEnumerable<T3> ThirdResult, IEnumerable<T4> FourthResult)> GetMultipleAsync<T1, T2, T3, T4>(
            IDataQuery query,
            CancellationToken token = default);
    ValueTask<(IEnumerable<T1> FirstResult, IEnumerable<T2> SecondResult, IEnumerable<T3> ThirdResult, IEnumerable<T4> FourthResult, IEnumerable<T5> FifthResult)> GetMultipleAsync<T1, T2, T3, T4, T5>(
        IDataQuery query,
        CancellationToken token = default);
    DynamicParameters BuildArgs(IEnumerable<KeyValuePair<string, object>> parameters);
}
