namespace Remy.Gambit.Core.Generics;

public class PaginatedList<T>
{
    public IEnumerable<T>? List { get; set; }

    public int TotalItems { get; set; }

    public int PageSize { get; set; }

    public int TotalPages
    {
        get
        {
            return (int)Math.Ceiling(TotalItems / ((PageSize==0 ? 1 : (double)(PageSize))) );
        }
    }
}
