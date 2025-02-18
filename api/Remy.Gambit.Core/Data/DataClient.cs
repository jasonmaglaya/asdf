using Dapper;
using Remy.Gambit.Core.Extensions;
using System.Data;

namespace Remy.Gambit.Core.Data;

public abstract class DataClient(IDbConnection dbConnection, int timeout = 10) : IDataClient
{
    protected readonly IDbConnection _dbConnection = dbConnection;
    protected readonly int _timeout = timeout;

    public virtual DynamicParameters BuildArgs(IEnumerable<KeyValuePair<string, object>> parameters)
    {
        var parameterList = new DynamicParameters();

        foreach (var parameter in parameters)
        {
            if (parameter.Value is null)
            {
                parameterList.Add(parameter.Key);
            }
            else if (parameter.Value.GetType().Name.Contains("AnonymousType"))
            {
                parameterList.AddDynamicParams(parameter.Value);
            }
            else if (parameter.Value is string)
            {
                parameterList.Add(parameter.Key, parameter.Value, DbType.String);
            }
            else
            {
                parameterList.Add(parameter.Key, parameter.Value);
            }
        }

        return parameterList;
    }

    public virtual async ValueTask<int> ExecuteAsync(IDataQuery command, CancellationToken token = default)
    {
        token.ThrowIfCancellationRequested();

        var args = BuildArgs(command.Parameters!);

        if(_dbConnection.State == ConnectionState.Closed)
        {
            _dbConnection.Open();
        }

        return await _dbConnection.ExecuteAsync(command.CmdText.ThrowIfNullOrEmpty(), args, commandTimeout: _timeout);
    }

    public virtual async ValueTask<int> ExecuteProcedureAsync(IDataQuery command, CancellationToken token = default)
    {
        token.ThrowIfCancellationRequested();

        var args = BuildArgs(command.Parameters!);

        if (_dbConnection.State == ConnectionState.Closed)
        {
            _dbConnection.Open();
        }

        return await _dbConnection.ExecuteAsync(command.CmdText.ThrowIfNullOrEmpty(), args, commandType: CommandType.StoredProcedure, commandTimeout: _timeout);
    }

    public virtual async ValueTask<T> ExecuteScalarAsync<T>(IDataQuery command, CancellationToken token = default)
    {
        token.ThrowIfCancellationRequested();

        var args = BuildArgs(command.Parameters!);

        if (_dbConnection.State == ConnectionState.Closed)
        {
            _dbConnection.Open();
        }

        return (await _dbConnection.ExecuteScalarAsync<T>(command.CmdText.ThrowIfNullOrEmpty(), args, commandTimeout: _timeout))!;
    }

    public virtual async ValueTask<T> ExecuteScalarProcedureAsync<T>(IDataQuery command, CancellationToken token = default)
    {
        token.ThrowIfCancellationRequested();

        var args = BuildArgs(command.Parameters!);

        if (_dbConnection.State == ConnectionState.Closed)
        {
            _dbConnection.Open();
        }

        var result = await _dbConnection.ExecuteScalarAsync<T>(command.CmdText.ThrowIfNullOrEmpty(), args, commandType: CommandType.StoredProcedure, commandTimeout: _timeout);
        return result!;
    }

    public virtual async ValueTask<IEnumerable<T>> GetCollectionAsync<T>(IDataQuery query, CancellationToken token = default)
    {
        token.ThrowIfCancellationRequested();

        var args = BuildArgs(query.Parameters!);

        if (_dbConnection.State == ConnectionState.Closed)
        {
            _dbConnection.Open();
        }

        return await _dbConnection.QueryAsync<T>(query.CmdText.ThrowIfNullOrEmpty(), args, commandTimeout: _timeout);
    }

    public virtual async ValueTask<T> GetFirstOrDefaultAsync<T>(IDataQuery query, CancellationToken token = default)
    {
        token.ThrowIfCancellationRequested();

        var args = BuildArgs(query.Parameters!);

        if (_dbConnection.State == ConnectionState.Closed)
        {
            _dbConnection.Open();
        }

        return (await _dbConnection.QueryFirstOrDefaultAsync<T>(query.CmdText.ThrowIfNullOrEmpty(), args, commandTimeout: _timeout))!;
    }

    public virtual async ValueTask<(IEnumerable<T1> FirstResult, IEnumerable<T2> SecondResult)> GetMultipleAsync<T1, T2>(
        IDataQuery query,
        CancellationToken token = default)
    {
        token.ThrowIfCancellationRequested();

        var args = BuildArgs(query.Parameters!);

        if (_dbConnection.State == ConnectionState.Closed)
        {
            _dbConnection.Open();
        }

        using var result = await _dbConnection.QueryMultipleAsync(query.CmdText.ThrowIfNullOrEmpty(), args, commandTimeout: _timeout);

        return (result.Read<T1>(), result.Read<T2>());
    }

    public virtual async ValueTask<(IEnumerable<T1> FirstResult, IEnumerable<T2> SecondResult, IEnumerable<T3> ThirdResult)> GetMultipleAsync<T1, T2, T3>(
        IDataQuery query,
        CancellationToken token = default)
    {
        token.ThrowIfCancellationRequested();

        var args = BuildArgs(query.Parameters!);

        if (_dbConnection.State == ConnectionState.Closed)
        {
            _dbConnection.Open();
        }

        using var result = await _dbConnection.QueryMultipleAsync(query.CmdText.ThrowIfNullOrEmpty(), args, commandTimeout: _timeout);

        return (result.Read<T1>(), result.Read<T2>(), result.Read<T3>());
    }

    public virtual async ValueTask<(IEnumerable<T1> FirstResult, IEnumerable<T2> SecondResult, IEnumerable<T3> ThirdResult, IEnumerable<T4> FourthResult)> GetMultipleAsync<T1, T2, T3, T4>(
        IDataQuery query,
        CancellationToken token = default)
    {
        token.ThrowIfCancellationRequested();

        var args = BuildArgs(query.Parameters!);

        if (_dbConnection.State == ConnectionState.Closed)
        {
            _dbConnection.Open();
        }

        using var result = await _dbConnection.QueryMultipleAsync(query.CmdText.ThrowIfNullOrEmpty(), args, commandTimeout: _timeout);

        return (result.Read<T1>(), result.Read<T2>(), result.Read<T3>(), result.Read<T4>());
    }

    public virtual async ValueTask<(IEnumerable<T1> FirstResult, IEnumerable<T2> SecondResult, IEnumerable<T3> ThirdResult, IEnumerable<T4> FourthResult, IEnumerable<T5> FifthResult)> GetMultipleAsync<T1, T2, T3, T4, T5>(
        IDataQuery query,
        CancellationToken token = default)
    {
        token.ThrowIfCancellationRequested();

        var args = BuildArgs(query.Parameters!);

        if (_dbConnection.State == ConnectionState.Closed)
        {
            _dbConnection.Open();
        }

        using var result = await _dbConnection.QueryMultipleAsync(query.CmdText.ThrowIfNullOrEmpty(), args, commandTimeout: _timeout);

        return (result.Read<T1>(), result.Read<T2>(), result.Read<T3>(), result.Read<T4>(), result.Read<T5>());
    }
}
