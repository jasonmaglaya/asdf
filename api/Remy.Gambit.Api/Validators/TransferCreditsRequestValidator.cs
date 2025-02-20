using FluentValidation;
using Remy.Gambit.Api.Handlers.Users.Command.Dto;

namespace Remy.Gambit.Api.Validators
{
    public class TransferCreditsRequestValidator : AbstractValidator<TransferCreditsRequest>
    {
        public TransferCreditsRequestValidator()
        {
            RuleFor(request => request.UserId).NotNull().NotEmpty().WithMessage("UserId is required");
            RuleFor(request => request.Requestor).NotNull().NotEmpty().WithMessage("Requestor is required");
            RuleFor(request => request.Amount).NotNull().WithMessage("Amount is required")
                .GreaterThan(0).WithMessage("Invalid Amount");
        }
    }
}
