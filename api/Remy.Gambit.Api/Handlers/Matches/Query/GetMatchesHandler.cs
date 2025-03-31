using AutoMapper;
using Remy.Gambit.Api.Handlers.Matches.Query.Dto;
using Remy.Gambit.Core.Cqs;
using Remy.Gambit.Core.Generics;
using Remy.Gambit.Data.Events;
using Remy.Gambit.Data.Matches;

namespace Remy.Gambit.Api.Handlers.Matches.Query
{
    public class GetMatchesHandler(IMatchesRepository matchesRepository, IEventsRepository eventsRepository, IMapper mapper) : IQueryHandler<GetMatchesRequest, GetMatchesResult>
    {
        public async ValueTask<GetMatchesResult> HandleAsync(GetMatchesRequest request, CancellationToken token = default)
        {
            var result = await matchesRepository.GetMatchesAsync(request.EventId, request.PageNumber, request.PageSize, token);

            var @event = await eventsRepository.GetEventByIdAsync(request.EventId, token);

            return new GetMatchesResult {
                IsSuccessful = true,
                Result = mapper.Map<PaginatedList<Api.Dto.MatchListItem>>(result),
                EventStatus = @event.Status 
            };
        }
    }
}
