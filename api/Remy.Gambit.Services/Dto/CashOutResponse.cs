namespace Remy.Gambit.Services.Dto;

public class CashOutResponse
{
    public string? Type { get; set; }
    public required string Token { get; set; }
    public required string UserName { get; set; }
    public decimal Balance { get; set; }
    public required string Currency { get; set; }
    public string? Status { get; set; }
}