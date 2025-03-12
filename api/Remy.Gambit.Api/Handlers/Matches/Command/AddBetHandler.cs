using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.SignalR;
using Remy.Gambit.Api.Constants;
using Remy.Gambit.Api.Handlers.Matches.Command.Dto;
using Remy.Gambit.Api.Hubs;
using Remy.Gambit.Core.Concurrency;
using Remy.Gambit.Core.Cqs;
using Remy.Gambit.Data.Events;
using Remy.Gambit.Data.Matches;
using Remy.Gambit.Data.Users;
using Remy.Gambit.Models;

namespace Remy.Gambit.Api.Handlers.Matches.Command
{
    public class AddBetHandler(
        IValidator<AddBetRequest> validator,
        IMatchesRepository matchesRepository,
        IUsersRepository usersRepository,
        IEventsRepository eventsRepository,
        IHubContext<EventHub> matchHub,
        IMapper mapper,
        IUserLockService userLockService
        ) : ICommandHandler<AddBetRequest, AddBetResult>
    {
        private readonly IValidator<AddBetRequest> _validator = validator;
        private readonly IMatchesRepository _matchesRepository = matchesRepository;
        private readonly IUsersRepository _usersRepository = usersRepository;
        private readonly IEventsRepository _eventsRepository = eventsRepository;
        private readonly IHubContext<EventHub> _matchHub = matchHub;
        private readonly IMapper _mapper = mapper;
        private readonly IUserLockService _userLockService = userLockService;
        private readonly SemaphoreSlim _semaphore = new(1);

        public async ValueTask<AddBetResult> HandleAsync(AddBetRequest command, CancellationToken token = default)
        {
            var validationResult = await _validator.ValidateAsync(command, token);

            if (!validationResult.IsValid)
            {
                return new AddBetResult { IsSuccessful = false, ValidationResults = validationResult.Errors.Select(x => x.ErrorMessage) };
            }

            
            var match = await _matchesRepository.GetMatchAsync(command.MatchId, token);

            if (match is null)
            {
                return new AddBetResult { IsSuccessful = false, ValidationResults = ["Invalid Match ID"] };
            }

            // Check if match is open
            if (match.Status != MatchStatuses.Open)
            {
                return new AddBetResult { IsSuccessful = false, ValidationResults = ["Match is already closed"] };
            }

            var @event = await _eventsRepository.GetEventByIdAsync(match.EventId, token);

            if (@event is null)
            {
                return new AddBetResult { IsSuccessful = false, ValidationResults = ["Invalid Event ID"] };
            }

            // Check if event is active
            if (@event?.Status != EventStatuses.Active)
            {
                return new AddBetResult { IsSuccessful = false, ValidationResults = ["Invalid Event ID"] };
            }

            if (command.TeamCode == Config.Draw)
            {
                // Check if amount is within limits
                if (command.Amount < @event.MinDrawBet || (command.Amount > @event.MaxDrawBet && @event.MaxDrawBet > 0m))
                {
                    return new AddBetResult { IsSuccessful = false, ValidationResults = ["Invalid amount"] };
                }

                try
                {
                    await _semaphore.WaitAsync(token);

                    var (totalBets, _) = await _matchesRepository.GetTotalBetsAsync(command.MatchId, token);

                    var totalDraw = totalBets.Where(x => x.Code == Config.Draw).Select(x => x.Amount).FirstOrDefault();

                    if (@event.MaxDrawBet > 0m && totalDraw + command.Amount > @event.MaxDrawBet)
                    {
                        return new AddBetResult { IsSuccessful = false, ValidationResults = ["Invalid amount"] };
                    }
                }
                catch
                {
                    throw;
                }
                finally
                {
                    _semaphore.Release();
                }
            }
            else
            {
                // Check if amount is within limits
                if (command.Amount < @event.MinimumBet || (command.Amount > @event.MaximumBet && @event.MaximumBet > 0m))
                {
                    return new AddBetResult { IsSuccessful = false, ValidationResults = ["Invalid amount"] };
                }
            }            

            try
            {
                var lockAcquired = await _userLockService.AcquireLockAsync(command.UserId);
                if (!lockAcquired)
                {
                    return new AddBetResult { IsSuccessful = false, ValidationResults = ["User is locked"] };
                }

                var user = await _usersRepository.GetUserByIdAsync(command.UserId, token);

                if (user == null)
                {
                    return new AddBetResult { IsSuccessful = false, Errors = ["Invalid UserId"] };
                }

                // Check if user is allowed to bet
                if (!user.IsActive || user.IsBettingLocked)
                {
                    return new AddBetResult { IsSuccessful = false, ValidationResults = ["User is not allowed to bet"] };
                }

                // Check if user has enough credits
                if (user.Credits < command.Amount)
                {
                    return new AddBetResult { IsSuccessful = false, ValidationResults = ["Insufficient credits"] };
                }

                var bets = await _matchesRepository.GetBetsByUserIdAsync(command.MatchId, command.UserId, token);

                var totalBet = bets.Where(x => x.Code == command.TeamCode).Select(x => x.Amount).FirstOrDefault() ?? 0;

                // Check if total bet is within limits
                if (totalBet > @event.MaximumBet)
                {
                    return new AddBetResult { IsSuccessful = false, ValidationResults = ["Invalid amount"] };
                }

                var credits = await _matchesRepository.AddBetAsync(command.UserId, command.MatchId, command.TeamCode!, command.Amount, command.IpAddress!, token);
                
                bets = await _matchesRepository.GetBetsByUserIdAsync(command.MatchId, command.UserId, token);
                var betsDto = _mapper.Map<IEnumerable<Api.Dto.TotalBet>>(bets);

                return new AddBetResult { IsSuccessful = true, Credits = credits.GetValueOrDefault(), Bets = betsDto };
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                await _userLockService.ReleaseLockAsync(command.UserId);

                var totalBets = await _matchesRepository.GetTotalBetsAsync(command.MatchId, token);
                await _matchHub.Clients.Group(@event.Id.ToString()).SendAsync(EventHubEvents.TotalBetsReceived, totalBets.Item1, cancellationToken: token);
            }
        }
    }
}
