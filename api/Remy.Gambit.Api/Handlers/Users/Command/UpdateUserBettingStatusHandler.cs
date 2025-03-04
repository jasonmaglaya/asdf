using FluentValidation;
using Remy.Gambit.Api.Handlers.Users.Command.Dto;
using Remy.Gambit.Core.Cqs;
using Remy.Gambit.Data.Features;
using Remy.Gambit.Data.Users;

namespace Remy.Gambit.Api.Handlers.Users.Command
{
    public class UpdateUserBettingStatusHandler : ICommandHandler<UpdateUserBettingStatusRequest, UpdateUserBettingStatusResult>
    {
        private readonly IUsersRepository _usersRepository;
        private readonly IFeaturesRepository _featuresRepository;
        private readonly IValidator<UpdateUserBettingStatusRequest> _validator;

        public UpdateUserBettingStatusHandler(IUsersRepository usersRepository, IFeaturesRepository featuresRepository, IValidator<UpdateUserBettingStatusRequest> validator)
        {
            _usersRepository = usersRepository;
            _featuresRepository = featuresRepository;
            _validator = validator;
        }

        public async ValueTask<UpdateUserBettingStatusResult> HandleAsync(UpdateUserBettingStatusRequest command, CancellationToken token = default)
        {
            var validationResult = await _validator.ValidateAsync(command, token);
            if (!validationResult.IsValid)
            {
                return new UpdateUserBettingStatusResult { IsSuccessful = false, Errors = validationResult.Errors.Select(x => x.ErrorMessage) };
            }

            var user = await _usersRepository.GetUserByIdAsync(command.UserId, token);

            var requestor = await _usersRepository.GetUserByIdAsync(command.Requestor, token);
            var features = await _featuresRepository.GetFeaturesByRoleAsync(requestor.Role, token);

            if (user.Upline != requestor.Id && !features.Contains(Constants.Features.ActivateUser))
            {
                return new UpdateUserBettingStatusResult { IsSuccessful = false };
            }

            var result = await _usersRepository.UpdateBettingStatusAsync(command.UserId, command.IsBettingLocked, token);

            return new UpdateUserBettingStatusResult { IsSuccessful = result };
        }
    }
}
