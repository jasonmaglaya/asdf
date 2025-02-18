using FluentValidation;
using Remy.Gambit.Api.Handlers.Users.Query.Dto;

namespace Remy.Gambit.Api.Validators
{
    public class GetUserRequestValidator : AbstractValidator<GetUserRequest>
    {
        public GetUserRequestValidator()
        {
            RuleFor(request => request.UserId).NotNull().WithMessage("UserId is required");
            RuleFor(request => request.Requestor).NotNull().NotEmpty().WithMessage("Requestor is required");
        }
    }
}
