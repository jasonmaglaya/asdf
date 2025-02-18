using FluentValidation;
using Remy.Gambit.Api.Handlers.Auth.Query.Dto;

namespace Remy.Gambit.Api.Validators
{
    public class CheckUsernameRequestValidator : AbstractValidator<CheckUsernameRequest>
    {
        public CheckUsernameRequestValidator()
        {
            RuleFor(request => request.Username).NotNull().NotEmpty().WithMessage("Username is required");
        }
    }
}
