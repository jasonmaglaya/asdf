namespace Remy.Gambit.Models;

public class Role
{
    public required string Code { get; set; }
    public required string Name { get; set; }
    public bool IsActive { get; set; }
    public bool IsAgent { get; set; }
}
