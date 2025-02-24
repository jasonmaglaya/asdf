namespace Remy.Gambit.Models;

public class EventSummaryItem
{
    public int MatchNumber { get; set; }
    public string? BetOn { get; set; }
    public decimal Bet { get; set; }
    public DateTime BetTimeStamp { get; set; }
    public string? Winners { get; set; }
    public decimal Odds { get; set; }
    public decimal GainLoss { get; set; }
    public DateTime GainLossDate { get; set; }
    public string? Notes { get; set; }
}
