using AutoMapper;
using FluentValidation;
using Remy.Gambit.Api.Constants;
using Remy.Gambit.Api.Handlers.Events.Command.Dto;
using Remy.Gambit.Core.Cqs;
using Remy.Gambit.Data.Events;
using Remy.Gambit.Data.Matches;
using Remy.Gambit.Data.Teams;
using Remy.Gambit.Models;

namespace Remy.Gambit.Api.Handlers.Events.Command;

public class AddEventHandler(IEventsRepository eventsRepository, ITeamsRepository teamsRepository, IMatchesRepository matchesRepository, IMapper mapper, IValidator<AddEventRequest> validator) : ICommandHandler<AddEventRequest, AddEventResult>
{
    private readonly IEventsRepository _eventsRepository = eventsRepository;
    private readonly ITeamsRepository _teamsRepository = teamsRepository;
    private readonly IMatchesRepository _matchesRepository = matchesRepository;
    private readonly IMapper _mapper = mapper;
    private readonly IValidator<AddEventRequest> _validator = validator;

    public async ValueTask<AddEventResult> HandleAsync(AddEventRequest command, CancellationToken token = default)
    {
        var validationResult = await _validator.ValidateAsync(command, token);
        if (!validationResult.IsValid)
        {
            return new AddEventResult { IsSuccessful = false, Errors = validationResult.Errors.Select(x => x.ErrorMessage) };
        }

        // Add event
        var @event = _mapper.Map<Event>(command.Event);
        var eventId = await _eventsRepository.AddEventAsync(@event, token);

        // Add teams
        foreach (var team in command.Teams)
        {
            var teamId = await _teamsRepository.AddTeamAsync(eventId, _mapper.Map<Team>(team), token);            
        }

        // Add initial match
        var match = new Match
        {
            EventId = eventId,
            Number = 1,
            Sequence = 1,
            Status = MatchStatuses.New
        };

        await _matchesRepository.AddMatchAsync(eventId, match, token);

        return new AddEventResult { EventId = eventId, IsSuccessful = true };
    }
}
