using AutoMapper;
using Remy.Gambit.Api.Handlers.Matches.Query.Dto;
using Remy.Gambit.Core.Cqs;
using Remy.Gambit.Data.Matches;

namespace Remy.Gambit.Api.Handlers.Matches.Query
{
    public class GetMatchHandler(IMatchesRepository matchesRepository, IMapper mapper) : IQueryHandler<GetMatchRequest, GetMatchResult>
    {
        private readonly IMatchesRepository _matchesRepository = matchesRepository;
        private readonly IMapper _mapper = mapper;

        public async ValueTask<GetMatchResult> HandleAsync(GetMatchRequest request, CancellationToken token = default)
        {
            var result = await _matchesRepository.GetMatchAsync(request.MatchId, token);

            return new GetMatchResult { IsSuccessful = true, Result = _mapper.Map<Api.Dto.Match>(result) };
        }
    }
}
