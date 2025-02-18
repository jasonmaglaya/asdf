using FluentValidation;
using Remy.Gambit.Api.Handlers.Events.Command.Dto;

namespace Remy.Gambit.Api.Validators
{
    public class NextMatchRequestValidator : AbstractValidator<NextMatchRequest>
    {
        public NextMatchRequestValidator()
        {
            RuleFor(request => request.EventId)
                .NotNull()
                .WithMessage("Event ID is required");
        }
    }
}
