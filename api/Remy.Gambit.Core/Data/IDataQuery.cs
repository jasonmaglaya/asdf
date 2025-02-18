namespace Remy.Gambit.Core.Data;

public interface IDataQuery
{
    IDictionary<string, object?> Parameters { get; }

    string CmdText { get; set; }
}
