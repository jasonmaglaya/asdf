using FluentValidation;
using Microsoft.Extensions.Configuration;
using Remy.Gambit.Api.Constants;
using Remy.Gambit.Api.Handlers.Auth.Command.Dto;
using Remy.Gambit.Api.Helpers;
using Remy.Gambit.Core.Caching;
using Remy.Gambit.Core.Cqs;
using Remy.Gambit.Data.Users;

namespace Remy.Gambit.Api.Handlers.Auth.Command
{
    public class LoginHandler(IConfiguration configuration, IValidator<LoginRequest> validator, IUsersRepository userRepository, ICacheService cache) : ICommandHandler<LoginRequest, LoginResult>
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

            // Set Session ID
            var sessionId = Guid.NewGuid().ToString();
            cache.Set($"{Config.SessionID}:{user.Id}", sessionId);

            // Generate JWT
            var accessToken = TokenHelper.GenerateToken(user, sessionId, _configuration);

            // Generate and Set Refresh Token
            var refreshToken = TokenHelper.GenerateToken(64);
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
