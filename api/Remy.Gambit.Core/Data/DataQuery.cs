namespace Remy.Gambit.Core.Data;

public abstract class DataQuery : IDataQuery
{
    private readonly Dictionary<string, object?> parameters = [];
    public IDictionary<string, object?> Parameters => parameters;
    public string CmdText { get; set; } = string.Empty;
}
