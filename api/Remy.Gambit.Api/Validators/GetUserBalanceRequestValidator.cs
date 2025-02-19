using FluentValidation;
using Remy.Gambit.Api.Handlers.Users.Query.Dto;

namespace Remy.Gambit.Api.Validators;

public class GetUserBalanceRequestValidator : AbstractValidator<GetUserBalanceRequest>
{
    public GetUserBalanceRequestValidator()
    {
        RuleFor(request => request.UserId).NotNull().WithMessage("UserId is required");
        RuleFor(request => request.UserToken).NotNull().WithMessage("UserToken is required");
    }
}
