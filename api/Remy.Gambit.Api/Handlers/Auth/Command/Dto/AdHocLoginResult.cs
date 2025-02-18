using Remy.Gambit.Core.Cqs;

namespace Remy.Gambit.Api.Handlers.Auth.Command.Dto;

public class AdHocLoginResult : ICommandResult
{
    public string? Type { get; set; }
    public string? Url { get; set; }
    public string? Status { get; set; }
    public string? Token { get; set; }
}
