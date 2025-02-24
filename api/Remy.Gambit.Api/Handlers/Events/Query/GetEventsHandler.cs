using AutoMapper;
using Remy.Gambit.Api.Handlers.Events.Query.Dto;
using Remy.Gambit.Core.Cqs;
using Remy.Gambit.Core.Generics;
using Remy.Gambit.Data.Events;
using Remy.Gambit.Models;

namespace Remy.Gambit.Api.Handlers.Events.Query
{
    public class GetEventsHandler(IEventsRepository eventsRepository, IMapper mapper) : IQueryHandler<GetEventsRequest, GetEventsResult>
    {
        private readonly IEventsRepository _eventsRepository = eventsRepository;
        private readonly IMapper _mapper = mapper;

        public async ValueTask<GetEventsResult> HandleAsync(GetEventsRequest request, CancellationToken token = default)
        {
            var result = await _eventsRepository.GetEventsAsync(request.Status, request.PageNumber, request.PageSize, request.IncludeNew, token);

            return new GetEventsResult {IsSuccessful=true, Result = _mapper.Map<PaginatedList<Event>, PaginatedList<Api.Dto.EventListItem>>(result) };
        }
    }
}
