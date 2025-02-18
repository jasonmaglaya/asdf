namespace Remy.Gambit.Core.Cqs;

public abstract class QueryResult<T> : IQueryResult
{
    public IEnumerable<string> Errors { get; set; } = [];
    public IEnumerable<string> ValidationResults { get; set; } = [];
    public bool IsSuccessful { get; set; }
    public T? Result { get; set; }
}
