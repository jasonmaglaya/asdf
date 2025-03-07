using FluentValidation;
using Microsoft.Extensions.Configuration;
using Remy.Gambit.Api.Handlers.Auth.Command.Dto;
using Remy.Gambit.Api.Helpers;
using Remy.Gambit.Core.Cqs;
using Remy.Gambit.Data.Users;

namespace Remy.Gambit.Api.Handlers.Auth.Command;

public class RefreshTokenHandler : ICommandHandler<RefreshTokenRequest, RefreshTokenResult>
{
    private readonly IConfiguration _configuration;
    private readonly IValidator<RefreshTokenRequest> _validator;
    private readonly IUsersRepository _userRepository;

    public RefreshTokenHandler(IConfiguration configuration, IValidator<RefreshTokenRequest> validator, IUsersRepository userRepository)
    {
        _configuration = configuration;
        _validator = validator;
        _userRepository = userRepository;
    }

    public async ValueTask<RefreshTokenResult> HandleAsync(RefreshTokenRequest command, CancellationToken token = default)
    {
        var validationResult = await _validator.ValidateAsync(command, token);
        if (!validationResult.IsValid)
        {
            return new RefreshTokenResult { IsSuccessful = false, ValidationResults = validationResult.Errors.Select(x => x.ErrorMessage) };
        }

        var user = await _userRepository.GetUserByRefreshTokenAsync(command.RefreshToken, token);

        if (user is null || !user.IsActive)
        {
            return new RefreshTokenResult { IsSuccessful = false, ValidationResults = ["Unsuccessful"] };
        }

        var accessToken = TokenHelper.GenerateToken(user, command.RefreshToken, _configuration);
        var refreshTokenExpiry = DateTime.UtcNow.AddHours(_configuration.GetValue<int>("Jwt:RefreshTokenExpiryHours"));

        var refreshTokenIsSet = await _userRepository.UpdateRefreshTokenAsync(user.Id, command.RefreshToken, refreshTokenExpiry, token);

        if (!refreshTokenIsSet)
        {
            return new RefreshTokenResult { IsSuccessful = false, ValidationResults = ["Unsuccessful"] };
        }

        return new RefreshTokenResult
        {
            IsSuccessful = true,
            AccessToken = accessToken,
            RefreshToken = command.RefreshToken
        };
    }
}
