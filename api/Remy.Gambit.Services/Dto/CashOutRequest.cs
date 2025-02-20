namespace Remy.Gambit.Services.Dto;

public class CashOutRequest : RequestBase, IRequestDto
{
    public required string UserName { get; set; }
    public required string Amount { get; set; }
    public string? Currency { get; set; }
    public required string TransactionId { get; set; }
    public string? TableId { get; set; }
    public string? Round { get; set; }
}
