using FluentValidation;
using Microsoft.AspNetCore.SignalR;
using Remy.Gambit.Api.Constants;
using Remy.Gambit.Api.Handlers.Matches.Command.Dto;
using Remy.Gambit.Api.Hubs;
using Remy.Gambit.Core.Cqs;
using Remy.Gambit.Data.Events;
using Remy.Gambit.Data.Matches;

namespace Remy.Gambit.Api.Handlers.Matches.Command
{
    public class DeclareWinnerHandler(
        IValidator<DeclareWinnerRequest> validator,
        IMatchesRepository matchesRepository,
        IHubContext<EventHub> matchHub,
        IEventsRepository eventsRepository
        ) : ICommandHandler<DeclareWinnerRequest, DeclareWinnerResult>
    {
        private readonly IValidator<DeclareWinnerRequest> _validator = validator;
        private readonly IMatchesRepository _matchesRepository = matchesRepository;
        private readonly IHubContext<EventHub> _matchHub = matchHub;
        private readonly IEventsRepository _eventsRepository = eventsRepository;

        public async ValueTask<DeclareWinnerResult> HandleAsync(DeclareWinnerRequest command, CancellationToken token = default)
        {
            var validationResult = await _validator.ValidateAsync(command, token);
            if (!validationResult.IsValid)
            {
                return new DeclareWinnerResult { IsSuccessful = false, ValidationResults = validationResult.Errors.Select(x => x.ErrorMessage) };
            }

            var match = await _matchesRepository.GetMatchAsync(command.MatchId, token);

            if(match is null || match?.Status != MatchStatuses.Finalized)
            {
                return new DeclareWinnerResult { IsSuccessful = false, ValidationResults = ["Invalid Match ID"] };
            }

            var @event = await _eventsRepository.GetEventByIdAsync(match.EventId, token);

            if (@event.Status != EventStatuses.Active)
            {
                return new DeclareWinnerResult { IsSuccessful = false, ValidationResults = ["Invalid event ID"] };
            }

            await _matchesRepository.DeclareWinnerAsync(command.MatchId, command.TeamCodes, token);
            await _matchesRepository.UpdateStatusAsync(command.MatchId, MatchStatuses.Declared, token);

            await _matchHub.Clients.Group(match.EventId.ToString()).SendAsync(EventHubEvents.WinnerDeclared, command.TeamCodes, cancellationToken: token);
            await _matchHub.Clients.Group(match.EventId.ToString()).SendAsync(EventHubEvents.MatchStatusReceived, MatchStatuses.Declared, cancellationToken: token);

            return new DeclareWinnerResult { IsSuccessful = true };
        }
    }
}
