using AutoMapper;
using Remy.Gambit.Api.Dto;
using Remy.Gambit.Api.Handlers.Events.Query.Dto;
using Remy.Gambit.Core.Cqs;
using Remy.Gambit.Data.Events;

namespace Remy.Gambit.Api.Handlers.Events.Query;

public class GetEventSummaryHandler(IEventsRepository eventsRepository, IMapper mapper) : IQueryHandler<GetEventSummaryRequest, GetEventSummaryResult>
{
    private readonly IEventsRepository _eventsRepository = eventsRepository;
    private readonly IMapper _mapper = mapper;

    public async ValueTask<GetEventSummaryResult> HandleAsync(GetEventSummaryRequest request, CancellationToken token = default)
    {
        var result = await _eventsRepository.GetEventWinnersAsync(request.EventId, token);

        return new GetEventSummaryResult { IsSuccessful = true, Result = _mapper.Map<IEnumerable<EventSummaryItem>>(result) };
    }
}