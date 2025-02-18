using FluentValidation;
using Remy.Gambit.Api.Constants;
using Remy.Gambit.Api.Handlers.Auth.Query.Dto;
using Remy.Gambit.Core.Cqs;
using Remy.Gambit.Data.Users;

namespace Remy.Gambit.Api.Handlers.Auth.Query
{
    public class CheckReferralCodeHandler : IQueryHandler<CheckReferralCodeRequest, CheckReferralCodeResult>
    {
        private readonly IValidator<CheckReferralCodeRequest> _validator;
        private readonly IUsersRepository _usersRepository;

        public CheckReferralCodeHandler(IValidator<CheckReferralCodeRequest> validator, IUsersRepository usersRepository)
        {
            _validator = validator;
            _usersRepository = usersRepository;
        }

        public async ValueTask<CheckReferralCodeResult> HandleAsync(CheckReferralCodeRequest request, CancellationToken token = default)
        {
            var validationResult = await _validator.ValidateAsync(request, token);
            if (!validationResult.IsValid)
            {
                return new CheckReferralCodeResult { IsSuccessful = false, Errors = validationResult.Errors.Select(x => x.ErrorMessage) };
            }

            var user = await _usersRepository.GetUserByReferralCodeAsync(request.ReferralCode, token);

            var result = user is not null && user.UserRole is not null && user.UserRole.IsAgent;

            return new CheckReferralCodeResult { IsSuccessful = true, Result = result };
        }
    }
}
