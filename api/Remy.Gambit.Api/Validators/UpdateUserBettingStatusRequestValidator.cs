using FluentValidation;
using Remy.Gambit.Api.Handlers.Users.Command.Dto;

namespace Remy.Gambit.Api.Validators
{
    public class UpdateUserBettingStatusRequestValidator : AbstractValidator<UpdateUserBettingStatusRequest>
    {
        public UpdateUserBettingStatusRequestValidator()
        {
            RuleFor(request => request.UserId).NotNull().WithMessage("UserId is required");
            RuleFor(request => request.Requestor).NotNull().NotEmpty().WithMessage("Requestor is required");
        }
    }
}
