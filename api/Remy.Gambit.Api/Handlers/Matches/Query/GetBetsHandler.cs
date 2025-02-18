using AutoMapper;
using Remy.Gambit.Api.Handlers.Matches.Query.Dto;
using Remy.Gambit.Core.Cqs;
using Remy.Gambit.Data.Matches;

namespace Remy.Gambit.Api.Handlers.Matches.Query
{
    public class GetBetsHandler : IQueryHandler<GetBetsRequest, GetBetsResult>
    {
        private readonly IMatchesRepository _matchesRepository;
        private readonly IMapper _mapper;

        public GetBetsHandler(IMatchesRepository matchesRepository, IMapper mapper)
        {
            _matchesRepository = matchesRepository;
            _mapper = mapper;
        }

        public async ValueTask<GetBetsResult> HandleAsync(GetBetsRequest request, CancellationToken token = default)
        {
            var bets = await _matchesRepository.GetBetsAsync(request.MatchId, request.UserId, token);

            var betsDto = _mapper.Map<IEnumerable<Api.Dto.TotalBet>>(bets);

            return new GetBetsResult { IsSuccessful = true, Result = betsDto };
        }
    }
}
