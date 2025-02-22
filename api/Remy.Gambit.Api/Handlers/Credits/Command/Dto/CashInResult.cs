using Remy.Gambit.Core.Cqs;

namespace Remy.Gambit.Api.Handlers.Credits.Command.Dto;

public class CashInResult : CommandResult
{
    public string? Currency { get; set; }
    public decimal NewBalance { get; set; }
}