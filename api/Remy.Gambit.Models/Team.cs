namespace Remy.Gambit.Models;

public class Team
{
    public Guid EventId { get; set; }
    public string? Code { get; set; }
    public string? Name { get; set; }
    public string? Color { get; set; }
    public int? Sequence { get; set; }
}
