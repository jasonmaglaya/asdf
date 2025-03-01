using FluentValidation;
using Microsoft.AspNetCore.SignalR;
using Remy.Gambit.Api.Constants;
using Remy.Gambit.Api.Handlers.Matches.Command.Dto;
using Remy.Gambit.Api.Hubs;
using Remy.Gambit.Core.Cqs;
using Remy.Gambit.Data.Matches;

namespace Remy.Gambit.Api.Handlers.Matches.Command
{
    public class CancelMatchHandler : ICommandHandler<CancelMatchRequest, CancelMatchResult>
    {
        private readonly IValidator<CancelMatchRequest> _validator;
        private readonly IMatchesRepository _matchesRepository;
        private readonly IHubContext<EventHub> _matchHub;

        public CancelMatchHandler(
            IValidator<CancelMatchRequest> validator,
            IMatchesRepository matchesRepository,
            IHubContext<EventHub> matchHub
        )
        {
            _validator = validator;
            _matchesRepository = matchesRepository;
            _matchHub = matchHub;
        }

        public async ValueTask<CancelMatchResult> HandleAsync(CancelMatchRequest command, CancellationToken token = default)
        {
            var validationResult = await _validator.ValidateAsync(command, token);
            if (!validationResult.IsValid)
            {
                return new CancelMatchResult { IsSuccessful = false, Errors = validationResult.Errors.Select(x => x.ErrorMessage) };
            }

            var successful = await _matchesRepository.CancelMatchAsync(command.MatchId, command.CancelledBy, command.IpAddress!, token);

            await _matchHub.Clients.Group(command.EventId.ToString()).SendAsync(EventHubEvents.MatchStatusReceived, MatchStatuses.Cancelled, cancellationToken: token);

            return new CancelMatchResult { IsSuccessful = successful };
        }
    }
}
