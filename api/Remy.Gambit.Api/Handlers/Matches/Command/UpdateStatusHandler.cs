using FluentValidation;
using Microsoft.AspNetCore.SignalR;
using Remy.Gambit.Api.Constants;
using Remy.Gambit.Api.Handlers.Matches.Command.Dto;
using Remy.Gambit.Api.Hubs;
using Remy.Gambit.Core.Cqs;
using Remy.Gambit.Data.Matches;

namespace Remy.Gambit.Api.Handlers.Matches.Command
{
    public class UpdateStatusHandler : ICommandHandler<UpdateStatusRequest, UpdateStatusResult>
    {
        private readonly IValidator<UpdateStatusRequest> _validator;
        private readonly IMatchesRepository _matchesRepository;
        private readonly IHubContext<EventHub> _matchHub;

        public UpdateStatusHandler(
            IValidator<UpdateStatusRequest> validator,
            IMatchesRepository matchesRepository,
            IHubContext<EventHub> matchHub
        )
        {
            _validator = validator;
            _matchesRepository = matchesRepository;
            _matchHub = matchHub;
        }

        public async ValueTask<UpdateStatusResult> HandleAsync(UpdateStatusRequest command, CancellationToken token = default)
        {
            var validationResult = await _validator.ValidateAsync(command, token);
            if (!validationResult.IsValid)
            {
                return new UpdateStatusResult { IsSuccessful = false, Errors = validationResult.Errors.Select(x => x.ErrorMessage) };
            }

            var successful = await _matchesRepository.UpdateStatusAsync(command.MatchId, command.Status, token);

            if(successful)
            {
                await _matchHub.Clients.Group(command.EventId.ToString()).SendAsync(EventHubEvents.MatchStatusReceived, command.Status, cancellationToken: token);
            }

            return new UpdateStatusResult { IsSuccessful = successful };
        }
    }
}
