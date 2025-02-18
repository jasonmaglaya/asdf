using AutoMapper;
using Remy.Gambit.Api.Dto;
using Remy.Gambit.Api.Handlers.Events.Query.Dto;
using Remy.Gambit.Core.Cqs;
using Remy.Gambit.Data.Events;

namespace Remy.Gambit.Api.Handlers.Events.Query
{
    public class GetEventHandler : IQueryHandler<GetEventRequest, GetEventResult>
    {
        private readonly IEventsRepository _eventsRepository;
        private readonly IMapper _mapper;

        public GetEventHandler(IEventsRepository eventsRepository, IMapper mapper)
        {
            _eventsRepository = eventsRepository;
            _mapper = mapper;
        }

        public async ValueTask<GetEventResult> HandleAsync(GetEventRequest request, CancellationToken token = default)
        {
            var result = await _eventsRepository.GetEventByIdAsync(request.Id, token);
            return new GetEventResult {IsSuccessful=true, Result = _mapper.Map<Event>(result) };
        }
    }
}
