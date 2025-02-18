using FluentValidation;
using Microsoft.Extensions.Configuration;
using Remy.Gambit.Api.Handlers.Auth.Command.Dto;
using Remy.Gambit.Api.Helpers;
using Remy.Gambit.Core.Cqs;
using Remy.Gambit.Data.Users;

namespace Remy.Gambit.Api.Handlers.Auth.Command
{
    public class LoginHandler(IConfiguration configuration, IValidator<LoginRequest> validator, IUsersRepository userRepository) : ICommandHandler<LoginRequest, LoginResult>
    {
        private readonly IConfiguration _configuration = configuration;
        private readonly IValidator<LoginRequest> _validator = validator;
        private readonly IUsersRepository _userRepository = userRepository;

        public async ValueTask<LoginResult> HandleAsync(LoginRequest command, CancellationToken token = default)
        {
            var validationResult = await _validator.ValidateAsync(command, token);
            if (!validationResult.IsValid)
            {
                return new LoginResult { IsSuccessful = false, ValidationResults = validationResult.Errors.Select(x => x.ErrorMessage) };
            }

            var user = await _userRepository.GetUserByUsernameAsync(command.Username, token);

            if (user is null || !user.IsActive || !BCrypt.Net.BCrypt.Verify(command.Password, user.Password))
            {
                return new LoginResult { IsSuccessful = false, ValidationResults = ["Invalid username or password"] };
            }

            var accessToken = TokenHelper.GenerateToken(user, _configuration);
            var refreshToken = TokenHelper.GenerateRefreshToken();
            var refreshTokenExpiry = DateTime.UtcNow.AddHours(_configuration.GetValue("Jwt:RefreshTokenExpiryHours", 24));

            var refreshTokenIsSet = await _userRepository.UpdateRefreshTokenAsync(user.Id, refreshToken, refreshTokenExpiry, token);

            if (!refreshTokenIsSet)
            {
                return new LoginResult { IsSuccessful = false, Errors = ["Internal server error"] };
            }

            return new LoginResult
            {
                IsSuccessful = true,
                AccessToken = accessToken,
                RefreshToken = refreshToken
            };
        }
    }
}
