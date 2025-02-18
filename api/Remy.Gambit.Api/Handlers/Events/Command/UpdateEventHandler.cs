using AutoMapper;
using FluentValidation;
using Remy.Gambit.Api.Handlers.Events.Command.Dto;
using Remy.Gambit.Core.Cqs;
using Remy.Gambit.Data.Events;
using Remy.Gambit.Data.Teams;
using Remy.Gambit.Models;

namespace Remy.Gambit.Api.Handlers.Events.Command;

public class UpdateEventHandler(IEventsRepository eventsRepository, ITeamsRepository teamsRepository, IMapper mapper, IValidator<UpdateEventRequest> validator) : ICommandHandler<UpdateEventRequest, UpdateEventResult>
{
    private readonly IEventsRepository _eventsRepository = eventsRepository;
    private readonly ITeamsRepository _teamsRepository = teamsRepository;
    private readonly IMapper _mapper = mapper;
    private readonly IValidator<UpdateEventRequest> _validator = validator;

    public async ValueTask<UpdateEventResult> HandleAsync(UpdateEventRequest command, CancellationToken token = default)
    {
        var validationResult = await _validator.ValidateAsync(command, token);
        if (!validationResult.IsValid)
        {
            return new UpdateEventResult { IsSuccessful = false, Errors = validationResult.Errors.Select(x => x.ErrorMessage) };
        }

        // Add event
        var @event = _mapper.Map<Event>(command.Event);
        var eventId = await _eventsRepository.UpdateEventAsync(@event, token);

        // Update teams
        await _teamsRepository.DeleteTeamsByEventIdAsync(@event.Id, token);

        foreach (var team in command.Teams)
        {
            var teamId = await _teamsRepository.AddTeamAsync(@event.Id, _mapper.Map<Team>(team), token);            
        }

        return new UpdateEventResult { IsSuccessful = true };
    }
}
