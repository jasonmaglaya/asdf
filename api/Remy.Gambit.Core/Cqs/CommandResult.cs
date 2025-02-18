namespace Remy.Gambit.Core.Cqs;

public abstract class CommandResult : ICommandResult
{
    public IEnumerable<string> Errors { get; set; } = [];

    public IEnumerable<string> ValidationResults { get; set; } = [];

    public bool IsSuccessful { get; set; }

}
