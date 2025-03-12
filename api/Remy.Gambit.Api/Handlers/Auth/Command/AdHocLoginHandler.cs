using FluentValidation;
using Microsoft.Extensions.Configuration;
using Remy.Gambit.Api.Constants;
using Remy.Gambit.Api.Handlers.Auth.Command.Dto;
using Remy.Gambit.Api.Helpers;
using Remy.Gambit.Core.Caching;
using Remy.Gambit.Core.Cqs;
using Remy.Gambit.Data.ClientSecrets;
using Remy.Gambit.Data.Users;

namespace Remy.Gambit.Api.Handlers.Auth.Command;

public class AdHocLoginHandler(IConfiguration configuration, IValidator<AdHocLoginRequest> validator, IUsersRepository usersRepository, 
    IClientSecretsRepository clientSecretsRepository, ICacheService cache) : ICommandHandler<AdHocLoginRequest, AdHocLoginResult>
{
    private readonly SemaphoreSlim _semaphore = new(1);

    public async ValueTask<AdHocLoginResult> HandleAsync(AdHocLoginRequest command, CancellationToken token = default)
    {
        var validationResult = await validator.ValidateAsync(command, token);
        if (!validationResult.IsValid)
        {
            return new AdHocLoginResult { Type = command.Type, Status = "failure" };
        }

        var clientSecret = await clientSecretsRepository.GetClientSecretAsync(command.ClientId!, token);
        if (clientSecret is null)
        {
            return new AdHocLoginResult { Type = command.Type, Status = "failure" };
        }

        if (clientSecret.Secret != command.ClientSecret)
        {
            return new AdHocLoginResult { Type = command.Type, Status = "failure" };
        }

        if (!string.IsNullOrEmpty(clientSecret.IpAddress))
        {
            if (!clientSecret.IpAddress.Split(",").Any(x => x == command.IpAddress))
            {
                return new AdHocLoginResult { Type = command.Type, Status = "failure" };
            }
        }

        Models.User user;

        try
        {
            await _semaphore.WaitAsync(token);

            user = await usersRepository.GetUserByUsernameAsync(command.UserName!, token);
            if (user is not null && (user.Role != Constants.Roles.Player || !user.IsActive))
            {
                return new AdHocLoginResult { Type = command.Type, Status = "failure" };
            }

            if (user is null)
            {
                var succeeded = await usersRepository.SignUpAsync(command.UserName!, null, null, Constants.Roles.Player, null, null, true, token);

                if (!succeeded)
                {
                    return new AdHocLoginResult { Type = command.Type, Status = "failure" };
                }
                                
                user = await usersRepository.GetUserByUsernameAsync(command.UserName!, token);
                await usersRepository.UpdateStatusAsync(user.Id, true, token);
            }
        }
        catch
        {
            return new AdHocLoginResult { Type = command.Type, Status = "failure" };
        }
        finally
        {
            _semaphore.Release();
        }

        // Set Session ID
        var sessionId = Guid.NewGuid().ToString();
        cache.Set($"{Config.SessionID}:{user.Id}", sessionId);

        var accessToken = TokenHelper.GenerateToken(user, sessionId, configuration);

        var refreshTokenExpiry = DateTime.UtcNow.AddHours(configuration.GetValue("Jwt:RefreshTokenExpiryHours", 24));
        var refreshToken = TokenHelper.GenerateToken(64);

        var refreshTokenIsSet = await usersRepository.UpdateRefreshTokenAsync(user.Id, refreshToken, refreshTokenExpiry, token);
        if (!refreshTokenIsSet)
        {
            return new AdHocLoginResult { Type = command.Type, Status = "failure" };
        }

        var operatorToken = TokenHelper.GenerateToken(24);

        var callBackUrl = $"{configuration["MG:CallbackUrl"]}?accessToken={accessToken}&refreshToken={refreshToken}&operatorToken={operatorToken}";

        return new AdHocLoginResult
        {
            Type = command.Type,
            Status = "success",
            Token = operatorToken,
            Url = callBackUrl
        };
    }
}
