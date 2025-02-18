using FluentValidation;
using Remy.Gambit.Api.Handlers.Auth.Query.Dto;
using Remy.Gambit.Core.Cqs;
using Remy.Gambit.Data.Users;

namespace Remy.Gambit.Api.Handlers.Auth.Query
{
    public class CheckUsernameHandler : IQueryHandler<CheckUsernameRequest, CheckUsernameResult>
    {
        private readonly IValidator<CheckUsernameRequest> _validator;
        private readonly IUsersRepository _userRepository;

        public CheckUsernameHandler(IValidator<CheckUsernameRequest> validator, IUsersRepository userRepository)
        {
            _validator = validator;
            _userRepository = userRepository;
        }

        public async ValueTask<CheckUsernameResult> HandleAsync(CheckUsernameRequest request, CancellationToken token = default)
        {
            var validationResult = await _validator.ValidateAsync(request, token);
            if (!validationResult.IsValid)
            {
                return new CheckUsernameResult { IsSuccessful = false, Errors = validationResult.Errors.Select(x => x.ErrorMessage) };
            }

            var isUsernameAvailable = await _userRepository.CheckUsernameAvailabilityAsync(request.Username, token);

            return new CheckUsernameResult { IsSuccessful = true, Result = isUsernameAvailable };
        }
    }
}
