using FluentValidation;
using Remy.Gambit.Api.Handlers.Matches.Command.Dto;

namespace Remy.Gambit.Api.Validators
{
    public class DeclareWinnerRequestValidator : AbstractValidator<DeclareWinnerRequest>
    {
        public DeclareWinnerRequestValidator()
        {
            RuleFor(request => request.MatchId)
                .NotNull()
                .WithMessage("MatchId is required");

            RuleFor(request => request.TeamCodes)
                .NotNull()
                .NotEmpty()
                .WithMessage("TeamCodes is required");
        }
    }
}
