using FluentValidation;
using Remy.Gambit.Api.Handlers.Users.Command.Dto;
using Remy.Gambit.Core.Cqs;
using Remy.Gambit.Data.Features;
using Remy.Gambit.Data.Users;

namespace Remy.Gambit.Api.Handlers.Users.Command
{
    public class UpdateUserStatusHandler : ICommandHandler<UpdateUserStatusRequest, UpdateUserStatusResult>
    {
        private readonly IUsersRepository _usersRepository;
        private readonly IFeaturesRepository _featuresRepository;
        private readonly IValidator<UpdateUserStatusRequest> _validator;

        public UpdateUserStatusHandler(IUsersRepository usersRepository, IFeaturesRepository featuresRepository, IValidator<UpdateUserStatusRequest> validator)
        {
            _usersRepository = usersRepository;
            _featuresRepository = featuresRepository;
            _validator = validator;
        }

        public async ValueTask<UpdateUserStatusResult> HandleAsync(UpdateUserStatusRequest command, CancellationToken token = default)
        {
            var validationResult = await _validator.ValidateAsync(command, token);
            if (!validationResult.IsValid)
            {
                return new UpdateUserStatusResult { IsSuccessful = false, Errors = validationResult.Errors.Select(x => x.ErrorMessage) };
            }

            var user = await _usersRepository.GetUserByIdAsync(command.UserId, token);

            var requestor = await _usersRepository.GetUserByIdAsync(command.Requestor, token);
            var features = await _featuresRepository.GetFeaturesByRoleAsync(requestor.Role, token);

            if (user.Upline != requestor.Id && !features.Contains(Constants.Features.ActivateUser))
            {
                return new UpdateUserStatusResult { IsSuccessful = false };
            }

            var result = await _usersRepository.UpdateStatusAsync(command.UserId, command.IsActive, token);

            return new UpdateUserStatusResult { IsSuccessful = result };
        }
    }
}
