using Remy.Gambit.Core.Cqs;

namespace Remy.Gambit.Api.Handlers.Auth.Command.Dto
{
    public class LogoutRequest : ICommand
    {
        public required Guid UserId { get; set; }
    }
}
