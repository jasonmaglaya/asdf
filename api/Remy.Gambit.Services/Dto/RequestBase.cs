namespace Remy.Gambit.Services.Dto;

public abstract class RequestBase
{
    public string? Type { get; set; }
    public required string Token { get; set; }
}
