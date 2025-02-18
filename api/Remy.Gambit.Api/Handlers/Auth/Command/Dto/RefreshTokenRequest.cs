using Remy.Gambit.Core.Cqs;

namespace Remy.Gambit.Api.Handlers.Auth.Command.Dto;

public class RefreshTokenRequest : ICommand
{
    public required string RefreshToken { get; set; }
}
