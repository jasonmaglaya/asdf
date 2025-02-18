using Remy.Gambit.Api.Handlers.Auth.Command.Dto;
using Remy.Gambit.Core.Cqs;
using Remy.Gambit.Data.Users;
using System.Net.Http;
using System.Security.Claims;

namespace Remy.Gambit.Api.Handlers.Auth.Command
{
    public class LogoutHandler : ICommandHandler<LogoutRequest, LogoutResult>
    {
        private readonly IUsersRepository _userRepository;

        public LogoutHandler(IUsersRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async ValueTask<LogoutResult> HandleAsync(LogoutRequest command, CancellationToken token = default)
        {
            var isRefreshTokenUnset = await _userRepository.UpdateRefreshTokenAsync(command.UserId, null, null, token);

            return new LogoutResult { IsSuccessful = isRefreshTokenUnset };
        }
    }
}
