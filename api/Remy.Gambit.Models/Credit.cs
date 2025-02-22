namespace Remy.Gambit.Models;

public class Credit
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public decimal Amount { get; set; }
    public decimal? RunningBalance { get; set; }
    public DateTime TransactionDate { get; set; }
    public string? TransactionType { get; set; }
    public Guid? BetId { get; set; }
    public string? Notes { get; set; }
    public string? IpAddress { get; set; }
    public Guid? CreditsFrom { get; set; }
    public Guid? CreditsTo { get; set; }
    public Guid? GroupTransactionId { get; set; }
}
