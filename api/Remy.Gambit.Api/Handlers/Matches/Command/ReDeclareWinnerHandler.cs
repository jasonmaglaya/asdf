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
    public class ReDeclareWinnerHandler(
        IValidator<ReDeclareWinnerRequest> validator,
        IMatchesRepository matchesRepository,
        IEventsRepository eventsRepository,
        IHubContext<EventHub> matchHub
        ) : ICommandHandler<ReDeclareWinnerRequest, ReDeclareWinnerResult>
    {
        private readonly IValidator<ReDeclareWinnerRequest> _validator = validator;
        private readonly IMatchesRepository _matchesRepository = matchesRepository;
        private readonly IEventsRepository _eventsRepository = eventsRepository;
        private readonly IHubContext<EventHub> _matchHub = matchHub;

        public async ValueTask<ReDeclareWinnerResult> HandleAsync(ReDeclareWinnerRequest command, CancellationToken token = default)
        {
            var validationResult = await _validator.ValidateAsync(command, token);
            if (!validationResult.IsValid)
            {
                return new ReDeclareWinnerResult { IsSuccessful = false, ValidationResults = validationResult.Errors.Select(x => x.ErrorMessage) };
            }

            var match = await _matchesRepository.GetMatchAsync(command.MatchId, token);

            if(match is null)
            {
                return new ReDeclareWinnerResult { IsSuccessful = false, ValidationResults = ["Invalid match ID"] };
            }

            if (!Array.Exists([MatchStatuses.Declared, MatchStatuses.Completed, MatchStatuses.Cancelled], x => x == match.Status!))
            {
                return new ReDeclareWinnerResult { IsSuccessful = false, ValidationResults = ["Invalid match ID"] };
            }

            var winners = await _matchesRepository.GetMatchWinnersAsync(command.MatchId, token);

            // Validate is the team codes are the same as the winners
            if (winners.Count() == command.TeamCodes.Count() && !winners.Select(x => x.TeamCode).Except(command.TeamCodes).Any())
            {
                return new ReDeclareWinnerResult { IsSuccessful = false, ValidationResults = ["Invalid team codes"] };
            }

            var @event = await _eventsRepository.GetEventByIdAsync(match.EventId, token);

            if(@event.Status != EventStatuses.Active)
            {
                return new ReDeclareWinnerResult { IsSuccessful = false, ValidationResults = ["Invalid event ID"] };
            }

            await _matchesRepository.ReDeclareWinnerAsync(command.MatchId, command.TeamCodes, command.UserId, token);

            await _matchHub.Clients.Group(match.EventId.ToString()).SendAsync(EventHubEvents.WinnerReDeclared, command.TeamCodes, cancellationToken: token);

            if (@event.CurrentMatch == match.Id)
            {
                await _matchHub.Clients.Group(match.EventId.ToString()).SendAsync(EventHubEvents.WinnerDeclared, command.TeamCodes, cancellationToken: token);                
            }

            return new ReDeclareWinnerResult { IsSuccessful = true };
        }
    }
}
