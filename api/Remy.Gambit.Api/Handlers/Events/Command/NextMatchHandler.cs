using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.SignalR;
using Remy.Gambit.Api.Constants;
using Remy.Gambit.Api.Dto;
using Remy.Gambit.Api.Handlers.Events.Command.Dto;
using Remy.Gambit.Api.Hubs;
using Remy.Gambit.Core.Cqs;
using Remy.Gambit.Data.Events;

namespace Remy.Gambit.Api.Handlers.Events.Command
{
    public class NextMatchHandler(
        IValidator<NextMatchRequest> validator,
        IEventsRepository eventsRepository,
        IMapper mapper,
        IHubContext<EventHub> matchHub
        ) : ICommandHandler<NextMatchRequest, NextMatchResult>
    {
        private readonly IValidator<NextMatchRequest> _validator = validator;
        private readonly IEventsRepository _eventsRepository = eventsRepository;
        private readonly IMapper _mapper = mapper;
        private readonly IHubContext<EventHub> _matchHub = matchHub;

        public async ValueTask<NextMatchResult> HandleAsync(NextMatchRequest command, CancellationToken token = default)
        {
            var validationResult = await _validator.ValidateAsync(command, token);
            if (!validationResult.IsValid)
            {
                return new NextMatchResult { IsSuccessful = false, Errors = validationResult.Errors.Select(x => x.ErrorMessage) };
            }

            await _eventsRepository.AddMatchAsync(command.EventId, token);

            var result = await _eventsRepository.GetCurrentMatchAsync(command.EventId, token);

            var match = _mapper.Map<Match>(result.Item1);
            match.Teams = _mapper.Map<IEnumerable<Team>>(result.Item2);

            await _matchHub.Clients.Group(command.EventId.ToString()).SendAsync(EventHubEvents.NextMatchReceived, match, cancellationToken: token);

            return new NextMatchResult { IsSuccessful = true, Match = match };
        }
    }
}
