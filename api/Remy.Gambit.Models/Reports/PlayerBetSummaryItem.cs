namespace Remy.Gambit.Models.Reports;

public class PlayerBetSummaryItem
{
    public Guid UserId { get; set; }
    public string? TeamCode { get; set; }
    public decimal Amount { get; set; }
    public DateTime BetTimeStamp { get; set; }
    public string? IpAddress { get; set; }
}
