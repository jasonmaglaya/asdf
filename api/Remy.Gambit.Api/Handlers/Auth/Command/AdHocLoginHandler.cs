using FluentValidation;
using Microsoft.Extensions.Configuration;
using Remy.Gambit.Api.Handlers.Auth.Command.Dto;
using Remy.Gambit.Api.Helpers;
using Remy.Gambit.Core.Cqs;
using Remy.Gambit.Data.ClientSecrets;
using Remy.Gambit.Data.Users;

namespace Remy.Gambit.Api.Handlers.Auth.Command;

public class AdHocLoginHandler(IConfiguration configuration, IValidator<AdHocLoginRequest> validator, IUsersRepository usersRepository, 
    IClientSecretsRepository clientSecretsRepository) : ICommandHandler<AdHocLoginRequest, AdHocLoginResult>
{
    private readonly IConfiguration _configuration = configuration;
    private readonly IValidator<AdHocLoginRequest> _validator = validator;
    private readonly IUsersRepository _usersRepository = usersRepository;
    private readonly IClientSecretsRepository _clientSecretsRepository = clientSecretsRepository;
    private readonly SemaphoreSlim _semaphore = new(1);

    public async ValueTask<AdHocLoginResult> HandleAsync(AdHocLoginRequest command, CancellationToken token = default)
    {
        var validationResult = await _validator.ValidateAsync(command, token);
        if (!validationResult.IsValid)
        {
            return new AdHocLoginResult { Type = command.Type, Status = "failure" };
        }

        //var clientSecret = await _clientSecretsRepository.GetClientSecretAsync(command.ClientId!, token);
        //if (clientSecret is null)
        //{
        //    return new AdHocLoginResult { IsSuccessful = false, Errors = ["Invalid client secret or ID"] };
        //}

        //if (clientSecret.Secret != command.ClientSecret)
        //{
        //    return new AdHocLoginResult { IsSuccessful = false, Errors = ["Invalid client secret or ID"] };
        //}

        //if (!string.IsNullOrEmpty(clientSecret.IpAddress))
        //{
        //    if (clientSecret.IpAddress != command.IpAddress)
        //    {
        //        return new AdHocLoginResult { IsSuccessful = false, Errors = [$"Invalid client secret or ID - {command.IpAddress}"] };
        //    }
        //}



        Models.User user;

        try
        {
            await _semaphore.WaitAsync(token);

            user = await _usersRepository.GetUserByUsernameAsync(command.UserName!, token);
            if (user is not null && (user.Role != Constants.Roles.Player || !user.IsActive))
            {
                return new AdHocLoginResult { Type = command.Type, Status = "failure" };
            }

            if (user is null)
            {
                var succeeded = await _usersRepository.SignUpAsync(command.UserName!, null, null, Constants.Roles.Player, null, null, true, token);

                if (!succeeded)
                {
                    return new AdHocLoginResult { Type = command.Type, Status = "failure" };
                }
                                
                user = await _usersRepository.GetUserByUsernameAsync(command.UserName!, token);
                await _usersRepository.UpdateStatusAsync(user.Id, true, token);
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
        
        var accessToken = TokenHelper.GenerateToken(user, _configuration);
        var refreshToken = TokenHelper.GenerateRefreshToken();
        var refreshTokenExpiry = DateTime.UtcNow.AddHours(_configuration.GetValue("Jwt:RefreshTokenExpiryHours", 24));

        var refreshTokenIsSet = await _usersRepository.UpdateRefreshTokenAsync(user.Id, refreshToken, refreshTokenExpiry, token);
        if (!refreshTokenIsSet)
        {
            return new AdHocLoginResult { Type = command.Type, Status = "failure" };
        }

        var callBackUrl = $"{_configuration["MG:CallbackUrl"]}?accessToken={accessToken}&refreshToken={refreshToken}";

        return new AdHocLoginResult
        {
            Type = command.Type,
            Status = "success",
            Token = accessToken,
            Url = callBackUrl
        };
    }
}
