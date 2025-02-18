using FluentValidation;
using Remy.Gambit.Api.Handlers.Matches.Command.Dto;

namespace Remy.Gambit.Api.Validators
{
    public class CancelMatchRequestValidator : AbstractValidator<CancelMatchRequest>
    {
        public CancelMatchRequestValidator()
        {
            RuleFor(request => request.EventId)
                .NotNull()
                .WithMessage("EventId is required");

            RuleFor(request => request.MatchId)
                .NotNull()
                .WithMessage("MatchId is required");
        }
    }
}
