using FluentValidation;
using Remy.Gambit.Api.Handlers.Auth.Command.Dto;
using Remy.Gambit.Core.Cqs;
using Remy.Gambit.Data.Users;

namespace Remy.Gambit.Api.Handlers.Auth.Command;

public class ChangePasswordHandler (IUsersRepository usersRepository, IValidator<ChangePasswordRequest> changePasswordRequestValidator) : ICommandHandler<ChangePasswordRequest, ChangePasswordResult>
{
    public async ValueTask<ChangePasswordResult> HandleAsync(ChangePasswordRequest command, CancellationToken token = default)
    {
        var validationResult = await changePasswordRequestValidator.ValidateAsync(command, token);
        if (!validationResult.IsValid)
        {
            return new ChangePasswordResult { IsSuccessful = false, ValidationResults = validationResult.Errors.Select(x => x.ErrorMessage) };
        }

        var segments = command.UserId.ToString().Split('-');
        var securityCode = $"{segments[1][..2]}{segments[2][..2]}{segments[3][..2]}";

        if(command.SecurityCode != securityCode)
        {
            return new ChangePasswordResult
            {
                IsSuccessful = false,
                ValidationResults = ["Invalid security code"]
            };
        }

        var user = await usersRepository.GetUserByIdAsync(command.UserId, token);

        if (command.Role == Constants.Roles.Player || user.Role == Constants.Roles.Player)
        {
            return new ChangePasswordResult
            {
                IsSuccessful = false,
                ValidationResults = ["Invalid operation"]
            };
        }

        if (user is null || !user.IsActive || !BCrypt.Net.BCrypt.Verify(command.OldPassword, user.Password))
        {
            return new ChangePasswordResult
            {
                IsSuccessful = false,
                ValidationResults = ["Old password is not correct"]
            };
        }

        var newPassword = BCrypt.Net.BCrypt.HashPassword(command.NewPassword!);

        await usersRepository.ChangePasswordAsync(command.UserId, newPassword, token);

        return new ChangePasswordResult
        {
            IsSuccessful = true
        };
    }
}