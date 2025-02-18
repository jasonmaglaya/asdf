using AutoMapper;
using Remy.Gambit.Api.Dto;
using Remy.Gambit.Api.Handlers.Events.Query.Dto;
using Remy.Gambit.Core.Cqs;
using Remy.Gambit.Data.Events;

namespace Remy.Gambit.Api.Handlers.Events.Query;

public class GetEventWinnersHandler(IEventsRepository eventsRepository, IMapper mapper) : IQueryHandler<GetEventWinnersRequest, GetEventWinnersResult>
{
    private readonly IEventsRepository _eventsRepository = eventsRepository;
    private readonly IMapper _mapper = mapper;

    public async ValueTask<GetEventWinnersResult> HandleAsync(GetEventWinnersRequest request, CancellationToken token = default)
    {
        var result = await _eventsRepository.GetEventWinnersAsync(request.EventId, token);
        return new GetEventWinnersResult { IsSuccessful = true, Result = _mapper.Map<IEnumerable<Winner>>(result) };
    }
}