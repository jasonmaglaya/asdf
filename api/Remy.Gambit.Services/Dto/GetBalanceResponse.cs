namespace Remy.Gambit.Services.Dto;

public class GetBalanceResponse
{
    public string? Type { get; set; }
    public string? UserName { get; set; }
    public string? Status { get; set; }
    public string? Currency { get; set; }
    public decimal Balance { get; set; }
}
