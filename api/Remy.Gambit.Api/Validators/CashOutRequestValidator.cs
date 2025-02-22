using FluentValidation;
using Remy.Gambit.Api.Handlers.Credits.Command.Dto;

namespace Remy.Gambit.Api.Validators;

public class CashOutRequestValidator : AbstractValidator<CashOutRequest>
{
    public CashOutRequestValidator()
    {
        RuleFor(request => request.UserId).NotNull().NotEmpty().WithMessage("UserId is required");
        RuleFor(request => request.PartnerToken).NotNull().NotEmpty().WithMessage("PartnerToken is required");
        RuleFor(request => request.Amount).GreaterThan(0).WithMessage("Invalid Amount");
        RuleFor(request => request.Currency).NotNull().NotEmpty().WithMessage("Currency is required");
    }
}
