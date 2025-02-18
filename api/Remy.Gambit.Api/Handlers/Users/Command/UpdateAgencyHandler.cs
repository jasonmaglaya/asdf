using FluentValidation;
using Remy.Gambit.Api.Constants;
using Remy.Gambit.Api.Handlers.Users.Command.Dto;
using Remy.Gambit.Core.Cqs;
using Remy.Gambit.Data.Features;
using Remy.Gambit.Data.Users;

namespace Remy.Gambit.Api.Handlers.Users.Command
{
    public class UpdateAgencyHandler : ICommandHandler<UpdateAgencyRequest, UpdateAgencyResult>
    {
        private readonly IUsersRepository _usersRepository;
        private readonly IFeaturesRepository _featuresRepository;
        private readonly IValidator<UpdateAgencyRequest> _validator;

        public UpdateAgencyHandler(IUsersRepository usersRepository, IFeaturesRepository featuresRepository, IValidator<UpdateAgencyRequest> validator)
        {
            _usersRepository = usersRepository;
            _featuresRepository = featuresRepository;
            _validator = validator;
        }

        public async ValueTask<UpdateAgencyResult> HandleAsync(UpdateAgencyRequest command, CancellationToken token = default)
        {
            var validationResult = await _validator.ValidateAsync(command, token);
            if (!validationResult.IsValid)
            {
                return new UpdateAgencyResult { IsSuccessful = false, Errors = validationResult.Errors.Select(x => x.ErrorMessage) };
            }

            var user = await _usersRepository.GetUserByIdAsync(command.UserId, token);

            var requestor = await _usersRepository.GetUserByIdAsync(command.Requestor, token);
            var features = await _featuresRepository.GetFeaturesByRoleAsync(requestor.Role, token);

            if (user.Upline != requestor.Id && !features.Contains(Constants.Features.ActivateAgency))
            {
                return new UpdateAgencyResult { IsSuccessful = false };
            }

            var agentCode = user.AgentCode;
            var role = user.Role;

            var upline = await _usersRepository.GetUserByIdAsync(user.Upline.GetValueOrDefault(), token);

            if (upline is not null)
            {
                if (command.Commission >= upline.Commission)
                {
                    return new UpdateAgencyResult { IsSuccessful = false };
                }

                if (string.IsNullOrEmpty(agentCode))
                {
                    agentCode = AgentCodes.DownLines[upline.AgentCode!];
                }

                role = Constants.Roles.DownLines[upline.Role];                
            }
            else
            {
                if (features.Contains(Constants.Features.ActivateAgency))
                {
                    agentCode = AgentCodes.Incorporator;
                    role = Constants.Roles.Incorporator;
                }
            }
            
            var result = await _usersRepository.UpdateAgencyAsync(command.UserId, agentCode!, command.Commission.GetValueOrDefault(), role, token);

            return new UpdateAgencyResult { IsSuccessful = result };
        }
    }
}
