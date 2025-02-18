using Remy.Gambit.Core.Cqs;

namespace Remy.Gambit.Api.Handlers.Auth.Command.Dto;

public class LoginResult : CommandResult
{
    public string? AccessToken { get; set; }
    public string? RefreshToken { get; set; }
}
