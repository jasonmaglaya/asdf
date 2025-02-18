using FluentValidation;
using Remy.Gambit.Api.Constants;
using Remy.Gambit.Api.Handlers.Users.Command.Dto;
using Remy.Gambit.Core.Cqs;
using Remy.Gambit.Data.Users;

namespace Remy.Gambit.Api.Handlers.Users.Command
{
    public class UpdateUserRoleHandler : ICommandHandler<UpdateUserRoleRequest, UpdateUserRoleResult>
    {
        private readonly IUsersRepository _usersRepository;
        private readonly IValidator<UpdateUserRoleRequest> _validator;

        public UpdateUserRoleHandler(IUsersRepository usersRepository, IValidator<UpdateUserRoleRequest> validator)
        {
            _usersRepository = usersRepository;
            _validator = validator;
        }

        public async ValueTask<UpdateUserRoleResult> HandleAsync(UpdateUserRoleRequest command, CancellationToken token = default)
        {
            var validationResult = await _validator.ValidateAsync(command, token);
            if (!validationResult.IsValid)
            {
                return new UpdateUserRoleResult { IsSuccessful = false, Errors = validationResult.Errors.Select(x => x.ErrorMessage) };
            }

            var user = await _usersRepository.GetUserByIdAsync(command.UserId, token);
            if(user.Upline is not null)
            {
                return new UpdateUserRoleResult { IsSuccessful = false };
            }

            var result = await _usersRepository.UpdateRoleAsync(command.UserId, command.RoleCode, token);

            return new UpdateUserRoleResult { IsSuccessful = result };
        }
    }
}
