using FluentValidation;
using Remy.Gambit.Api.Handlers.Events.Command.Dto;

namespace Remy.Gambit.Api.Validators;

public class UpdateEventStatusRequestValidator : AbstractValidator<UpdateEventStatusRequest>
{
    public UpdateEventStatusRequestValidator()
    {
        RuleFor(request => request.EventId).NotNull().NotEmpty().WithMessage("EventId is required");
        RuleFor(request => request.Status).NotNull().NotEmpty().WithMessage("Status is required");
    }
}
