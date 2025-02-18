using Remy.Gambit.Core.Cqs;

namespace Remy.Gambit.Api.Handlers.Auth.Command.Dto
{
    public class LoginRequest : ICommand
    {
        public required string Username { get; set; }
        public required string Password { get; set; }
    }
}
