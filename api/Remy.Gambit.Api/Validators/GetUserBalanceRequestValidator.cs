using FluentValidation;
using Remy.Gambit.Api.Handlers.Credits.Request.Dto;

namespace Remy.Gambit.Api.Validators;

public class GetUserBalanceRequestValidator : AbstractValidator<GetUserBalanceRequest>
{
    public GetUserBalanceRequestValidator()
    {
        RuleFor(request => request.UserId).NotNull().WithMessage("UserId is required");
        RuleFor(request => request.PartnerToken).NotNull().WithMessage("PartnerToken is required");
    }
}
