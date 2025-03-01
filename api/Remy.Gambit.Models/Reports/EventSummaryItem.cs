namespace Remy.Gambit.Models.Reports;

public class EventSummaryItem
{
    public Guid MatchId { get; set; }
    public int Number { get; set; }
    public string? Winners { get; set; }
    public DateTime DeclareDate { get; set; }
    public string? DeclaredBy { get; set; }
    public string? IpAddress { get; set; }
    public decimal TotalBets { get; set; }
    public decimal Commission { get; set; }
    public decimal TotalDraw { get; set; }
    public decimal TotalDrawPayout { get; set; }
    public decimal DrawNet { get; set; }
}
