using FluentValidation;
using Remy.Gambit.Api.Handlers.Users.Query.Dto;

namespace Remy.Gambit.Api.Validators
{
    public class SearchUserRequestValidator : AbstractValidator<SearchUserRequest>
    {
        public SearchUserRequestValidator()
        {
            RuleFor(request => request.Keyword).NotNull().WithMessage("Keyword is required");
        }
    }
}
