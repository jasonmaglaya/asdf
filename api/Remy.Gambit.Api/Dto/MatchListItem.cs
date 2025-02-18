namespace Remy.Gambit.Api.Dto;

public class MatchListItem
{
    public Guid Id { get; set; }
    public Guid EventId { get; set; }
    public int Number { get; set; }
    public string? Description { get; set; }
    public string? Status { get; set; }
    public int? Sequence { get; set; }
    public string? WinnerCode { get; set; }
}