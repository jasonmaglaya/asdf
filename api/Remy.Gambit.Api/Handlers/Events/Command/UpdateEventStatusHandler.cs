using FluentValidation;
using Remy.Gambit.Api.Handlers.Events.Command.Dto;
using Remy.Gambit.Core.Cqs;
using Remy.Gambit.Data.Events;

namespace Remy.Gambit.Api.Handlers.Events.Command;

public class UpdateEventStatusHandler(IEventsRepository eventsRepository, IValidator<UpdateEventStatusRequest> validator) : ICommandHandler<UpdateEventStatusRequest, UpdateEventStatusResult>
{
    private readonly IEventsRepository _eventsRepository = eventsRepository;
    private readonly IValidator<UpdateEventStatusRequest> _validator = validator;

    public async ValueTask<UpdateEventStatusResult> HandleAsync(UpdateEventStatusRequest command, CancellationToken token = default)
    {
        var validationResult = await _validator.ValidateAsync(command, token);
        if (!validationResult.IsValid)
        {
            return new UpdateEventStatusResult { IsSuccessful = false, Errors = validationResult.Errors.Select(x => x.ErrorMessage) };
        }

        var result = await _eventsRepository.UpdateEventStatusAsync(command.EventId, command.Status, token);

        return new UpdateEventStatusResult { IsSuccessful = result };
    }
}
