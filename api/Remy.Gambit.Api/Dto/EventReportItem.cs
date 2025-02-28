namespace Remy.Gambit.Api.Dto;

public class EventReportItem
{
    public Guid EventId { get; set; }
    public string? Title { get; set; }
    public DateTime EventDate { get; set; }
    public int Matches { get; set; }
    public decimal CommissionPercentage { get; set; }
    public decimal TotalBets { get; set; }
    public decimal Commission { get; set; }
    public float DrawMultiplier { get; set; }
    public decimal TotalDraw { get; set; }
    public decimal TotalDrawPayout { get; set; }
    public decimal DrawNet { get; set; }
}
