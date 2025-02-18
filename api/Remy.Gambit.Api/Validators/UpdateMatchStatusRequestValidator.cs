using FluentValidation;
using Remy.Gambit.Api.Handlers.Matches.Command.Dto;

namespace Remy.Gambit.Api.Validators
{
    public class UpdateMatchStatusRequestValidator : AbstractValidator<UpdateStatusRequest>
    {
        public UpdateMatchStatusRequestValidator()
        {
            RuleFor(request => request.EventId)
                .NotNull()
                .WithMessage("EventId is required");

            RuleFor(request => request.MatchId)
                .NotNull()
                .WithMessage("MatchId is required");

            RuleFor(request => request.Status)
                .NotNull()
                .NotEmpty()
                .WithMessage("Status is required");
        }
    }
}
