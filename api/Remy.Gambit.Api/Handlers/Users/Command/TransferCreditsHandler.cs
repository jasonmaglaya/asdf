using FluentValidation;
using Remy.Gambit.Api.Handlers.Users.Command.Dto;
using Remy.Gambit.Core.Cqs;
using Remy.Gambit.Data.Features;
using Remy.Gambit.Data.Users;

namespace Remy.Gambit.Api.Handlers.Users.Command
{
    public class TransferCreditsHandler(IUsersRepository usersRepository, IFeaturesRepository featuresReporsitory, IValidator<TransferCreditsRequest> validator) : ICommandHandler<TransferCreditsRequest, TransferCreditsResult>
    {
        private readonly IUsersRepository _usersRepository = usersRepository;
        private readonly IFeaturesRepository _featuresReporsitory = featuresReporsitory;
        private readonly IValidator<TransferCreditsRequest> _validator = validator;

        public async ValueTask<TransferCreditsResult> HandleAsync(TransferCreditsRequest command, CancellationToken token = default)
        {
            var validationResult = await _validator.ValidateAsync(command, token);
            if (!validationResult.IsValid)
            {
                return new TransferCreditsResult { IsSuccessful = false, Errors = validationResult.Errors.Select(x => x.ErrorMessage) };
            }

            var requestor = await _usersRepository.GetUserByIdAsync(command.Requestor, token);
            var features = await _featuresReporsitory.GetFeaturesByRoleAsync(requestor.Role, token);

            Guid? from = null;
            if (command.From is not null)
            {
                if (!features.Contains(Constants.Features.TransferCredits))
                {
                    return new TransferCreditsResult { IsSuccessful = false };
                }

                var source = await _usersRepository.GetUserByIdAsync(command.From.Value, token);
                
                if(source is null || source.Credits < command.Amount)
                {
                    return new TransferCreditsResult { IsSuccessful = false };
                }

                from = source.Id;
            }
            else
            {
                if (!features.Contains(Constants.Features.PropagateCredits))
                {
                    if (requestor.Credits < command.Amount)
                    {
                        return new TransferCreditsResult { IsSuccessful = false };
                    }
                    
                    from = requestor.Id;
                }
            }

            var result = await _usersRepository.TransaferCreditsAsync(from, command.UserId, command.Amount, command.Requestor!, command.Notes, token);

            return new TransferCreditsResult { IsSuccessful = result };
        }
    }
}
