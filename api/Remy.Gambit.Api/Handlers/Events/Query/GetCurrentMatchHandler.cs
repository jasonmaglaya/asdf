using AutoMapper;
using Remy.Gambit.Api.Dto;
using Remy.Gambit.Api.Handlers.Events.Query.Dto;
using Remy.Gambit.Core.Cqs;
using Remy.Gambit.Data.Events;

namespace Remy.Gambit.Api.Handlers.Events.Query
{
    public class GetCurrentMatchHandler : IQueryHandler<GetCurrentMatchRequest, GetCurrentMatchResult>
    {
        private readonly IEventsRepository _eventsRepository;
        private readonly IMapper _mapper;

        public GetCurrentMatchHandler(IEventsRepository eventsRepository, IMapper mapper)
        {
            _eventsRepository = eventsRepository;
            _mapper = mapper;
        }

        public async ValueTask<GetCurrentMatchResult> HandleAsync(GetCurrentMatchRequest request, CancellationToken token = default)
        {
            var result = await _eventsRepository.GetCurrentMatchAsync(request.EventId, token);

            if (result.Item1 is null || !result.Item2.Any())
            {
                return new GetCurrentMatchResult { IsSuccessful = false };
            }

            var match = _mapper.Map<Match>(result.Item1);
            match.Teams = _mapper.Map<IEnumerable<Team>>(result.Item2);
            match.Winners = result.Item3?.Select(x => x.Code)!;

            return new GetCurrentMatchResult { IsSuccessful = true, Result = match };
        }
    }
}
