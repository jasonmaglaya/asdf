using FluentValidation;
using Remy.Gambit.Api.Handlers.Matches.Command.Dto;

namespace Remy.Gambit.Api.Validators
{
    public class AddBetRequestValidator : AbstractValidator<AddBetRequest>
    {
        public AddBetRequestValidator()
        {
            RuleFor(request => request.UserId).NotNull().WithMessage("UserId is required");
            RuleFor(request => request.MatchId).NotNull().NotEmpty().WithMessage("Match ID is required");
            RuleFor(request => request.TeamCode).NotNull().NotEmpty().WithMessage("Team Code is required");
            RuleFor(request => request.Amount).NotNull().NotEmpty().GreaterThan(0).WithMessage("Team Code is required");
        }
    }
}
