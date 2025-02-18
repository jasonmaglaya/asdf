using FluentValidation;
using Remy.Gambit.Api.Handlers.Matches.Command.Dto;

namespace Remy.Gambit.Api.Validators
{
    public class ReDeclareWinnerRequestValidator : AbstractValidator<ReDeclareWinnerRequest>
    {
        public ReDeclareWinnerRequestValidator()
        {
            RuleFor(request => request.MatchId)
                .NotNull()
                .WithMessage("MatchId is required");

            RuleFor(request => request.TeamCodes)
                .NotNull()
                .NotEmpty()
                .WithMessage("TeamCodes is required");

            RuleFor(request => request.UserId)
                .NotNull()
                .NotEmpty()
                .WithMessage("UserId is required");
        }
    }
}
