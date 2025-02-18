using FluentValidation;
using Remy.Gambit.Api.Handlers.Events.Command.Dto;

namespace Remy.Gambit.Api.Validators;

public class AddEventRequestValidator : AbstractValidator<AddEventRequest>
{
    public AddEventRequestValidator()
    {
        RuleFor(request => request.Event.Title).NotNull().WithMessage("Title is required");
        RuleFor(request => request.Event.MinimumBet).GreaterThan(0).LessThan(x => x.Event.MaximumBet).WithMessage("Minimum bet must be greater than 0 and less than the maximum bet.");
        RuleFor(request => request.Event.MaximumBet).GreaterThan(x => x.Event.MinimumBet).WithMessage("Maximum bet must be greater than minimum bet.");
        RuleFor(request => request.Event.MinDrawBet).GreaterThan(0).LessThan(x => x.Event.MaxDrawBet).WithMessage("Minimum bet for draw must be greater than 0 and less than the maximum bet  for draw.");
        RuleFor(request => request.Event.MaxDrawBet).GreaterThan(x => x.Event.MinDrawBet).WithMessage("Maximum bet for draw must be greater than minimum bet for draw.");
        RuleFor(request => request.Event.MaxWinners).GreaterThan(0).WithMessage("Maximum number of winners must be at least 1.");
        RuleFor(request => request.Event.Commission).GreaterThanOrEqualTo(0).LessThanOrEqualTo(1).WithMessage("Commission must be between 0 and 1.");
        RuleFor(request => request.Teams).NotNull().NotEmpty().WithMessage("Teams are required.");
        RuleFor(request => request.Teams.Count()).GreaterThan(1).WithMessage("Teams count must be at least 2.");
    }
}
