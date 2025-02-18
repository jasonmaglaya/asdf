namespace Remy.Gambit.Core.Cqs;

public interface IQueryResult
{
    IEnumerable<string> Errors { get; set; }

    IEnumerable<string> ValidationResults { get; set; }

    bool IsSuccessful { get; set; }
}
