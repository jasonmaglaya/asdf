using AutoMapper;
using Remy.Gambit.Api.Handlers.Matches.Query.Dto;
using Remy.Gambit.Core.Cqs;
using Remy.Gambit.Core.Generics;
using Remy.Gambit.Data.Matches;

namespace Remy.Gambit.Api.Handlers.Matches.Query
{
    public class GetMatchesHandler(IMatchesRepository matchesRepository, IMapper mapper) : IQueryHandler<GetMatchesRequest, GetMatchesResult>
    {
        private readonly IMatchesRepository _matchesRepository = matchesRepository;
        private readonly IMapper _mapper = mapper;

        public async ValueTask<GetMatchesResult> HandleAsync(GetMatchesRequest request, CancellationToken token = default)
        {
            var result = await _matchesRepository.GetMatchesAsync(request.EventId, request.PageNumber, request.PageSize, token);

            return new GetMatchesResult { IsSuccessful = true, Result = _mapper.Map<PaginatedList<Api.Dto.MatchListItem>>(result) };
        }
    }
}
